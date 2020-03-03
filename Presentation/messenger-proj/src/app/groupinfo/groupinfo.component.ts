import { UserService } from './../services/user.service';
import { ChatService, ChatContent } from './../services/chat.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../services/user.service';

@Component({
  selector: 'app-groupinfo',
  templateUrl: './groupinfo.component.html',
  styleUrls: ['./groupinfo.component.css']
})
export class GroupinfoComponent implements OnInit {


  chatContent:ChatContent=new ChatContent();

  admin:User=new User();

  constructor(private chatservice:ChatService) { }

  ngOnInit() {
    this.chatservice.currentChatContentSource.subscribe(data=>
      {
        console.log(data);
         this.chatContent=data;   
      })
  }

  getAdmin(){
    return this.chatContent.users.find(u=>u.id==this.chatContent.adminId);
  }

  getPhoto(){
     return this.chatservice.chats.value.find(c=>c.id==this.chatservice.currentChatId).photo;
  }

  public GetUrl(photo:string){
    return `${this.chatservice.photourl}/${photo}`; 
  }

}
