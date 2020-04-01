import { UserService } from './../services/user.service';
import { Chat } from './../services/chat.service';
import { Component, OnInit } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { User } from '../services/user.service';
import { WebdriverWebElement } from 'protractor/built/element';

@Component({
  selector: 'app-friendinfo',
  templateUrl: './friendinfo.component.html',
  styleUrls: ['./friendinfo.component.css']
})
export class FriendinfoComponent implements OnInit {

  currentChatUser:User=null;

  constructor(public chatservice:ChatService,public userservice:UserService) { }

  ngOnInit() {
    this.chatservice.currentChatUserSource.subscribe(user=>this.currentChatUser=user);
  }

  public GetUrl(id:number){
    return `${this.chatservice.photourl}/${this.chatservice.users.value.find(u=>u.id===id).photoName}`; 
  }

  turnAnim(){
    let elem=document.getElementById("friendinfophone");
    if(elem.classList.contains("phone")){
      elem.classList.add("phoneanim");
      elem.classList.remove("phone");
    }
    else{
      elem.classList.remove("phoneanim");
      elem.classList.add("phone");
    }
  }

  block(id:number){
    this.userservice.block(id);
  }

  unblock(id:number){
    this.userservice.unblock(id);
  }

  delete(id:number){
    this.chatservice.DeleteConversation(id)
    .then(()=>{
      document.getElementById("friendclose").click();
    });
  }
}
