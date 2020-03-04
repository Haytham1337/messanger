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
  constructor(private userservice:UserService,private chatservice:ChatService) { }

  ngOnInit() {
    this.chatservice.searchConvSource.subscribe((res)=>this.SearchConv=res);
  }

  search(){
    this.chatservice.SearchConversation(this.filter);
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
