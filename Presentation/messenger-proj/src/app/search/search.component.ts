import { UserService, User } from './../services/user.service';
import { Component, OnInit } from '@angular/core';
import { ChatService, SearchConversation } from '../services/chat.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {

  SearchConv:SearchConversation[];
  filter:string=null;
  constructor(public userservice:UserService,public chatservice:ChatService) { }

  ngOnInit() {
    this.chatservice.searchConvSource.subscribe((res)=>this.SearchConv=res);
  }

  search(){
    if(this.filter=="")
    {
      this.chatservice.SearchUpdate([]);
    }
    else{
      this.chatservice.SearchConversation(this.filter);
    }
  }

  createChat(id:number,type:number){
    if(type==0){
      this.chatservice.CreateChate(id);
    }
    else{
      this.chatservice.AddToGroup(id);
    }
    this.chatservice.SearchUpdate([]);
  }

}
