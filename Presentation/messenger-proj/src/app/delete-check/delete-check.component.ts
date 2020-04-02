import { ChatService } from './../services/chat.service';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-delete-check',
  templateUrl: './delete-check.component.html',
  styleUrls: ['./delete-check.component.css']
})
export class DeleteCheckComponent implements OnInit {

  @Input()
  id:number;
  constructor(public chatservice:  ChatService) { }

  ngOnInit() {
  }

  delete(){
    this.chatservice.DeleteConversation(this.id)
    .then(()=>{
      document.getElementById("closecheck").click();
    });  
  }
}
