import { PhotoService } from './photo.service';
import { ConfigService } from './config.service';
import { HttpClient ,HttpHeaders} from '@angular/common/http';
import { Injectable} from '@angular/core';
import * as signalR from "@aspnet/signalr"
import {DomSanitizer} from '@angular/platform-browser';
import { User } from './user.service';
import { BehaviorSubject } from 'rxjs';

export interface Message{
  messageId:number,
  content:string,
  userId:number,
  timeCreated:Date,
  chatId:number,
  photo:string,
  messagePhoto:string
}

export class ChatContent{
  name:string;
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
  Type:number,
  isOnline:boolean
  messageTime:Date;
}

export  class SearchConversation{
  id:number;
  name:string;
  photo:string;
  type:number;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection:signalR.HubConnection;

  error:boolean=false;

  private messages=new BehaviorSubject<Message[]>([]);
  messagessource=this.messages.asObservable();

  public users=new BehaviorSubject<User[]>([]);
  userssource=this.users.asObservable();

  public searchConversations=new BehaviorSubject<SearchConversation[]>([]);
  searchConvSource=this.searchConversations.asObservable();

  public chats=new BehaviorSubject<Chat[]>([]);
  chatssource=this.chats.asObservable();

  private currentChatUser=new BehaviorSubject<User>(null);
  currentChatUserSource=this.currentChatUser.asObservable();

  public currentChatContent=new BehaviorSubject<ChatContent>(null);
  currentChatContentSource=this.currentChatContent.asObservable();

  public currentChatId:number;

  public currentChatAdmin:number;

  public photourl:string;

  public  currentChatType:number;

  public portion:number=1;

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
    if(!chatid){
      return;
    }
    this.currentChatId=chatid;
    this.portion=1;
    let url = `${await this.config.getConfig("getchatmessages")}?id=${chatid}&portion=${this.portion}`;
    
    let photopath = await this.config.getConfig("photopath");
    this.photourl=photopath;

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');
            
    return await this.http.get<ChatContent>(url,{headers:headers}).toPromise()
        .then((data)=>{
         data.users=data.users.sort((fu,su)=>{
             if(fu.isOnline>su.isOnline){
               return -1;
             }
             else{
               return 1;
             }
          })
          console.log(data.messages);
          this. CurrentContentUpdate(data);
          this.currentChatType=data.type;
          this.currentChatAdmin=data.adminId;
          this.MessagesUpdate(data.messages);
          this.UsersUpdate(data.users);
        })
    }

    public async loadMessages(){
      if(!this.currentChatId){
        return;
      }
      let url = `${await this.config.getConfig("getchatmessages")}?id=${this.currentChatId}&portion=${this.portion+1}`;
    
    let photopath = await this.config.getConfig("photopath");
    this.photourl=photopath;

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');
            
    return await this.http.get<ChatContent>(url,{headers:headers}).toPromise()
        .then((data)=>{
          this.portion+=1;
          this.MessagesUpdate(this.messages.value.concat(data.messages));
    });
  }

  public sendMessage (data) {
    let now =new Date();
    let month=now.getMonth()+ 1;
    data.timeCreated= `${now.getFullYear()}-${month}-${now.getDate()} ${now.getHours()}:${now.getMinutes()}:${now.getSeconds()}`; 
    console.log(data);
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
        curchat.messageTime=new Date();
        this.messages.value.splice(0,0,data);
        this.MessagesUpdate(this.messages.getValue());
        this.ChatsUpdate(this.chats.getValue());
      }
      else{
        var curchat=this.chats.getValue()
            .find(c=>c.id==chatId);
        curchat.content=data.content;
        curchat.messageTime=new Date();
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

  SearchUpdate(conv:SearchConversation[]){
    this.searchConversations.next(conv);
  }

  CurrentContentUpdate(content:ChatContent){
    this.currentChatContent.next(content);
  }

  CurrentChatUserUpdate(user:User){
    if(user==null)
    {
      this.currentChatUser.next(user);
    }
    else{
      let chat=this.chats.value.find(chat=>chat.secondUserId==user.id);
      user.isblocked=chat.isBlocked;
      this.currentChatUser.next(user);
    }
  }

  public async CreateChate(id:number){
    let url=await this.config.getConfig("createchat");
    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.post(url,JSON.stringify({id: id}),{headers:headers}).subscribe(
      res=>{
        this.GetChats();
        this.Reconnect();
      },
      err=>{
        var chat=this.chats.getValue()
        .find(c=>c.secondUserId==id);
        this.currentChatId=chat.id;
        this.getMessages(this.currentChatId);
      }
    )
  }

  public async GetChats(leaveCurrent:boolean=false){
    let url=await this.config.getConfig("getchats");
    let imgpath=await this.config.getConfig("photopath");

    return await this.http.get<Chat[]>(url).toPromise()
      .then(res=>{
        if(res.length==0)
        {
          this.CurrentContentUpdate(null);
          this.ChatsUpdate([]);
          this.CurrentChatUserUpdate(null);
          let closeelem=document.getElementById("groupclose");
          if(closeelem!=null)
          {
            closeelem.click();
          }
          this.MessagesUpdate([]);
        }

        let mappedres= res.map(chat=>{
          chat.photo=`${imgpath}/${chat.photo}`;
          return chat;
        })

        if(!leaveCurrent&&mappedres.length>0){
          this.currentChatId=mappedres[0].id;
          this.getMessages(this.currentChatId);
          this.ChatsUpdate(res);
        }
        else{
          this.ChatsUpdate(res);
        }
        
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
    let url:string=null;
    if(data.IsChannel){
       url=await this.config.getConfig("createChannel");
    }
    else{
       url=await this.config.getConfig("createGroup");
    }

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.post(url,JSON.stringify(data),{headers:headers}).subscribe(
      res=>{
        this.error=false;
        this.GetChats();
        this.Reconnect();
        document.getElementById("closecreate").click();
      },
      err=>{
        this.error=true;
      }
    )
  }

  public async SearchConversation(filter:string){
    let url =await this.config.getConfig("searchconversation")+`?Filter=${filter}`;
    let imgpath=await this.config.getConfig("photopath");
    return await this.http.get<SearchConversation[]>(url).toPromise()
    .then(res=>
      {
        let mappedres= res.map(conv=>{
          conv.photo=`${imgpath}/${conv.photo}`;
          return conv;
        })

        console.log(mappedres);

        this.SearchUpdate(mappedres);
      });
  }

  public async AddToGroup(id:number){
    let url =await this.config.getConfig("subscribeForChannel");

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.post(url,JSON.stringify({id: id}),{headers:headers}).subscribe(
      res=>{
        this.GetChats(false);
        this.Reconnect();
      },
      err=>{
        var chat=this.chats.getValue()
        .find(c=>c.id==id);
        this.currentChatId=chat.id;
        this.getMessages(this.currentChatId);
      });
  }

  public async DeleteConversation(id:number){
    let url=`${await this.config.getConfig("delete")}?ConversationId=${id}`;

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.delete(url,{headers:headers}).subscribe(
      res=>{
        this.GetChats();
      },
      err=>{
        console.log("error");
  });

  }

  public async DeleteMessage(id:number){
    let url=`${await this.config.getConfig("deletemessage")}?MessageId=${id}`;

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.http.delete(url,{headers:headers}).subscribe(
      res=>{
        let messageToDelete= this.messages.value.find(mes=>mes.messageId==id);
        this.messages.value.splice(this.messages.value.indexOf(messageToDelete),1);
        this.MessagesUpdate(this.messages.value);
      },
      );

  }
}
