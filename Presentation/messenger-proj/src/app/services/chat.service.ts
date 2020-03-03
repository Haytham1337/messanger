import { PhotoService } from './photo.service';
import { ConfigService } from './config.service';
import { HttpClient ,HttpHeaders} from '@angular/common/http';
import { Injectable, OnInit, ɵConsole, ComponentFactoryResolver } from '@angular/core';
import * as signalR from "@aspnet/signalr"
import {DomSanitizer} from '@angular/platform-browser';
import { User } from './user.service';
import { BehaviorSubject } from 'rxjs';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

export interface Message{
  content:string,
  userId:number,
  timeCreated:Date,
  chatId:number
}

export class ChatContent{
  users:User[];
  messages:Message[];
  type:number;
  adminId;
}

export class Group{
  UsersId:number[];
  IsChannel:boolean;
  GroupName:string;
}

export interface Chat{
  id:number,
  photo:string,
  content:string,
  secondUserId:number,
  isBlocked,
  Type:number
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection:signalR.HubConnection;

  private messages=new BehaviorSubject<Message[]>([]);
  messagessource=this.messages.asObservable();

  public users=new BehaviorSubject<User[]>([]);
  userssource=this.users.asObservable();

  public chats=new BehaviorSubject<Chat[]>([]);
  chatssource=this.chats.asObservable();

  private currentChatUser=new BehaviorSubject<User>(null);
  currentChatUserSource=this.currentChatUser.asObservable();

  private currentChatContent=new BehaviorSubject<ChatContent>(null);
  currentChatContentSource=this.currentChatContent.asObservable();

  public currentChatId:number;

  public photourl:string;

  public  currentChatType:number;

  messagesUpdate = this.messages.asObservable();

  constructor(private http:HttpClient,private config:ConfigService,private sanitizer:DomSanitizer, private photo: PhotoService) { }

  startConnection=async()=>{
    const options: signalR.IHttpConnectionOptions = {
      transport:signalR.HttpTransportType.WebSockets
    };
    this.hubConnection = new signalR.HubConnectionBuilder()
                              .withUrl(`https://localhost:44334/chat/?token=${localStorage["token"]}`,options)
                              .build();

    this.hubConnection.start().then(()=>console.log("Connection started!!"));
  }

  public async getMessages(chatid:number){
    this.currentChatId=chatid;
    let url = `${await this.config.getConfig("getchatmessages")}?id=${chatid}`;
    
    let photopath = await this.config.getConfig("photopath");
    this.photourl=photopath;

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');
            
    return await this.http.get<ChatContent>(url,{headers:headers}).toPromise()
        .then((data)=>{
          this. CurrentContentUpdate(data);
          this.currentChatType=data.type;
          this.MessagesUpdate(data.messages);
          this.UsersUpdate(data.users);})
    }

  public sendMessage (data:Message) {
    data.chatId=this.currentChatId;
    this.hubConnection.invoke('SendToAll', data)
    .catch(err => console.error(err));
  }

  public updateChat = async () => {
    this.hubConnection.on('update', async (data,chatId) => {
      if(this.currentChatId==chatId){
        var curchat=this.chats.getValue()
            .find(c=>c.id==chatId);
        
        this.chats.getValue().splice(this.chats.getValue().indexOf(curchat),1);
        this.chats.getValue().splice(0,0,curchat);

        curchat.content=data.content;
        this.messages.value.push(data);
        this.MessagesUpdate(this.messages.getValue());
        this.ChatsUpdate(this.chats.getValue());
      }
    });
}

  MessagesUpdate(messages: Message[]) {
    this.messages.next(messages);
  }

  UsersUpdate(users:User[]){
    this.users.next(users);
    let currentchat=this.chats.value.find(chat=>chat.id==this.currentChatId);
    let currentUser=this.users.value.find(user=>user.id==currentchat.secondUserId);
    this.CurrentChatUserUpdate(currentUser);
  }

  ChatsUpdate(chats:Chat[]){
    this.chats.next(chats);
  }

  CurrentContentUpdate(content:ChatContent){
    this.currentChatContent.next(content);
  }

  CurrentChatUserUpdate(user:User){
    let chat=this.chats.value.find(chat=>chat.secondUserId==user.id);
    user.isblocked=chat.isBlocked;
    this.currentChatUser.next(user);
  }

  public async CreateChate(SecondUserId:number){
    let url=await this.config.getConfig("createchat");
    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.post(url,JSON.stringify({SecondUserId}),{headers:headers}).subscribe(
      res=>{
        this.GetChats();
        this.Reconnect();
      },
      err=>{
        var chat=this.chats.getValue()
        .find(c=>c.secondUserId==SecondUserId);
        this.currentChatId=chat.id;
        this.getMessages(this.currentChatId);
      }
    )
  }

  public async GetChats(){
    let url=await this.config.getConfig("getchats");
    let imgpath=await this.config.getConfig("photopath");

    return await this.http.get<Chat[]>(url).toPromise()
      .then(res=>{
        let mappedres= res.map(chat=>{
          chat.photo=`${imgpath}/${chat.photo}`;
          return chat;
        })

        this.currentChatId=mappedres[0].id;
        this.getMessages(this.currentChatId);
        this.ChatsUpdate(res);
        return res;
      })
  }

  public async UpdateChats(){
    let url=await this.config.getConfig("getchats");
    let imgpath=await this.config.getConfig("photopath");

    return await this.http.get<Chat[]>(url).toPromise()
      .then(res=>{
        let mappedres= res.map(chat=>{
          chat.photo=`${imgpath}/${chat.photo}`;
          return chat;
        })
        this.getMessages(this.currentChatId);
        this.ChatsUpdate(res);
        return res;
      })
  }

  public Reconnect(){
    this.hubConnection.stop()
    .then(()=>this.hubConnection.start());
  }

  public async CreateGroup(data:Group){
    let url=await this.config.getConfig("creategroup");

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.post(url,JSON.stringify(data),{headers:headers}).subscribe(
      res=>{
        this.GetChats();
        this.Reconnect();
      },
      err=>{
        console.log(err);
      }
    )
  }
}
