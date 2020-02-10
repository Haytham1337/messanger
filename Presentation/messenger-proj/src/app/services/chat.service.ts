import { PhotoService } from './photo.service';
import { ConfigService } from './config.service';
import { HttpClient ,HttpHeaders} from '@angular/common/http';
import { Injectable, OnInit, ɵConsole } from '@angular/core';
import * as signalR from "@aspnet/signalr"
import {DomSanitizer} from '@angular/platform-browser';
import { User } from './user.service';
import { BehaviorSubject } from 'rxjs';

export interface Message{
  content:string,
  userId:number,
  timeCreated:Date
}

export interface ChatContent{
  users:User[],
  messages:Message[]
}

export interface Chat{
  id:number,
  photo:string,
  content:string
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection:signalR.HubConnection;

  private messages=new BehaviorSubject<Message[]>([]);
  messagessource=this.messages.asObservable();

  private users=new BehaviorSubject<User[]>([]);
  userssource=this.users.asObservable();

  private chats=new BehaviorSubject<Chat[]>([]);
  chatssource=this.chats.asObservable();

  public photourl:string;

  messagesUpdate = this.messages.asObservable();

  constructor(private http:HttpClient,private config:ConfigService,private sanitizer:DomSanitizer, private photo: PhotoService) { }


  startConnection=async()=>{
    this.hubConnection = new signalR.HubConnectionBuilder()
                              .withUrl("https://localhost:44334/chat")
                              .build();
    this.hubConnection.start()
                    .then(()=>console.log("Connection started"))
                    .catch(err=>console.log(`error occured: ${err}`));                 
  }

  public async getMessages(chatid:number){
    let url = `${await this.config.getConfig("getchatmessages")}?id=${chatid}`;
    
    let photopath = await this.config.getConfig("photopath");
    this.photourl=photopath;

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');
            
    return await this.http.get<ChatContent>(url,{headers:headers}).toPromise()
        .then((data)=>{
          this.MessagesUpdate(data.messages);
          this.UsersUpdate(data.users);
          console.log(data);
          })
    }

  public sendMessage (data:Message) {
    this.hubConnection.invoke('SendToAll', data)
    .catch(err => console.error(err));
  }

  public updateChat = async () => {
    this.hubConnection.on('update', async (data) => {
      
      this.messages.value.push(data);
      this.MessagesUpdate(this.messages.getValue());
    });
}

  MessagesUpdate(messages: Message[]) {
    this.messages.next(messages);
  }

  UsersUpdate(users:User[]){
    this.users.next(users);
  }

  ChatsUpdate(chats:Chat[]){
    this.chats.next(chats);
  }

  public async CreateChate(SecondUserId:number){
    let url=await this.config.getConfig("createchat");

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.post(url,JSON.stringify({SecondUserId}),{headers:headers}).toPromise()
      .then(res=>{
        if(res===true){
          this.GetChats();
        }
        else{

        }
      });
  }

  public async GetChats(){
    let url=await this.config.getConfig("getchats");
    let imgpath=await this.config.getConfig("photopath");

    return await this.http.get<Chat[]>(url).toPromise()
      .then(res=>{
        console.log(res);
        let mappedres= res.map(chat=>{
          chat.photo=`${imgpath}/${chat.photo}`;
          return chat;
        })

        this.ChatsUpdate(res);
        return res;
      })
  }
}
