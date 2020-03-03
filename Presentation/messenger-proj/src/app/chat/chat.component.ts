import { PhotoService } from './../services/photo.service';
import { UserService, User } from './../services/user.service';
import { ChatService, Message } from './../services/chat.service';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, AfterViewInit } from '@angular/core';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {

  newMessage:string;

  currentUser:User=new User();

  messages:Message[]=null;

  users:User[]=null;

  currentChatUser:User=new User();

  constructor(private chatservice:ChatService,private userservice:UserService) 
  { 
    chatservice.messagesUpdate.subscribe(res=>this.messages=res);
  }

   ngOnInit() {  
    
    this.chatservice.startConnection();  
    
    this.chatservice.updateChat();

    this.chatservice.messagessource.subscribe(mess=>this.messages=mess);
    
    this.userservice.data.subscribe(user=>this.currentUser=user);

    this.chatservice.userssource.subscribe(users=>this.users=users);

    this.chatservice.currentChatUserSource.subscribe(user=>this.currentChatUser=user);
  }

  sendMessage(){
    this.chatservice.sendMessage({content:this.newMessage} as Message);
    this.newMessage=null;
  }

  public GetUrl(id:number){
    return `${this.chatservice.photourl}/${this.users.find(u=>u.id===id).photoName}`; 
  }

  public GetUser(id:number){
    return this.users.find(u=>u.id===id);
  }

}