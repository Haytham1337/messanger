import { CurrentDate } from './../pipes/date.pipe';
import { UserService, User } from './../services/user.service';
import { ChatService, Message } from './../services/chat.service';
import { Component, OnInit} from '@angular/core';
import {DatePipe} from '@angular/common';
import { ScrollEvent } from 'ngx-scroll-event';
import { PhotoService } from '../services/photo.service';


@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {

  newMessage:string;

  newPhoto:string=null;

  currentUser:User=new User();

  messages:Message[]=null;

  users:User[]=null;

  currentChatUser:User=new User();

  movedToTop:boolean=false;

  toggled: boolean = false;

  constructor(public chatservice:ChatService,public photoservice:PhotoService,public userservice:UserService, public datePipe:DatePipe,public curDate:CurrentDate) 
  { 
    chatservice.messagesUpdate.subscribe(res=>this.messages=res);
  }

  handleSelection(event) {
    if(this.newMessage==undefined){
      this.newMessage=event.char;
    }
    else{
      this.newMessage+=event.char;
    }
  }


   ngOnInit() {  
    
    this.chatservice.startConnection();  
    
    this.chatservice.updateChat();

    this.chatservice.messagessource.subscribe(mess=>
      {

        this.messages=mess;
        if(!this.movedToTop){
          setTimeout(()=>{
            let elem=document.getElementById("contentdiv");
            elem.scrollTop=elem.scrollHeight;
          },5)
        }

        this.movedToTop=false;
      });
    
    this.userservice.data.subscribe(user=>this.currentUser=user);

    this.photoservice.mesPhoto.subscribe(data=>{this.newPhoto=data;});

    this.chatservice.userssource.subscribe(users=>this.users=users);

    this.chatservice.currentChatUserSource.subscribe(user=>this.currentChatUser=user);
  }

  sendMessage(){
    this.chatservice.sendMessage({content:this.newMessage,photo:this.newPhoto} as Message);
    this.newMessage=null;
    this.photoservice.UpdateMesPhoto(null);
  }

  public GetUrl(photo:string){
    return `${this.chatservice.photourl}/${photo}`; 
  }

  public GetUser(id:number){
    return this.users.find(u=>u.id===id);
  }

  transformDate(date){
    let messageTime=new Date(date);

    let currentDate=new Date();

    if(currentDate.getDay()==messageTime.getDay()){
      return this.curDate.transform(date);
    }
    else{
      return this.datePipe.transform(date);
    }
  }

  scroll(){
    let elem=document.getElementById("contentdiv");
    if(elem.scrollTop==0){
      this.chatservice.loadMessages();
      this.movedToTop=true;
    }
  }

   photochosen(){
      document.getElementById("attach_photo_input").click();
    }

    photoselected(event){
      this.photoservice.UploadMessagePhoto(event.target.files[0]);
      event.target.value="";
    }

    deletePhoto(){
      this.photoservice.UpdateMesPhoto(null);
    }

    deleteMessage(id:number){
      this.chatservice.DeleteMessage(id);
    }
}