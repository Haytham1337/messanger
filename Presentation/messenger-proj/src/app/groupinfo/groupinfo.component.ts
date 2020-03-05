import { PhotoService } from './../services/photo.service';
import { UserService, User } from './../services/user.service';
import { ChatService, ChatContent } from './../services/chat.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-groupinfo',
  templateUrl: './groupinfo.component.html',
  styleUrls: ['./groupinfo.component.css']
})
export class GroupinfoComponent implements OnInit {


  chatContent:ChatContent=new ChatContent();

  admin:User=new User();

  constructor(private chatservice:ChatService,private photoser:PhotoService,private userservice:UserService) { }

  ngOnInit() {
    this.chatservice.currentChatContentSource.subscribe(data=>
      {
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

  async photoselected(event){
    (await this.photoser.UploadGroupPhoto(event.target.files[0],this.chatservice.currentChatId))
        .subscribe(res=>this.chatservice.GetChats(true))
  }

  photoCliked(){
    let elem=document.getElementById("group_photo_id");
    if(elem!=null)
    {
      elem.click();
    }
  }

  delete(id:number){
    this.chatservice.DeleteConversation(id)
    .then(()=>{
      document.getElementById("groupclose").click();
    });
  }

}
