import { Chat } from './../services/chat.service';
import { Component, OnInit } from '@angular/core';
import { ChatService } from '../services/chat.service';
import {DatePipe} from '@angular/common';
import { CurrentDate } from './../pipes/date.pipe';


@Component({
  selector: 'app-chatlist',
  templateUrl: './chatlist.component.html',
  styleUrls: ['./chatlist.component.css']
})
export class ChatlistComponent implements OnInit {

  chats:Chat[];

  constructor(private chatservice:ChatService,private datePipe:DatePipe,private curDate:CurrentDate) { }

  ngOnInit() {
    this.chatservice.GetChats();
    this.chatservice.chatssource.subscribe(res=>{
      this.chats=res;
    })
  }

  getMessages(id:number){
    this.chatservice.getMessages(id);
  }

  chatclick(event){
    console.log(event);
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

}
