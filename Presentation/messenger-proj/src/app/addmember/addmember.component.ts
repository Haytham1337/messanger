import { ChatService } from './../services/chat.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { Component, OnInit } from '@angular/core';
import { User, UserService } from '../services/user.service';

@Component({
  selector: 'app-addmember',
  templateUrl: './addmember.component.html',
  styleUrls: ['./addmember.component.css']
})
export class AddmemberComponent implements OnInit {

  SearchUsers:User[];
  selectedItems :User [];
  dropdownSettings:IDropdownSettings = {};
  idToAdd:number;

  constructor(public userservice:UserService,public chatservice:ChatService) { }

  ngOnInit() {

    this.userservice.searchdata.subscribe((res)=>this.SearchUsers=res);

    this.dropdownSettings= {
      singleSelection: true,
      idField: 'id',
      textField: 'email',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true,
      enableCheckAll:false,
      defaultOpen:false,
      allowRemoteDataSearch:true,
      clearSearchFilter:true,
      closeDropDownOnSelection:true
    };
  }

  add(user:User){
    this.idToAdd=user.id;
  }


 onFilterChange(filter:string) {
   if(filter=="")
   {
     this.userservice.updateSearchUsers([]);
   }
   else{
     this.userservice.SearchUsers(filter);
   }
 }

 AddMember(){
   this.userservice.AddMember(this.idToAdd,this.chatservice.currentChatId)
   .then(()=>{
     this.userservice.updateSearchUsers([]);
     this.selectedItems=[];
     setTimeout(()=>{
      this.userservice.erroradd=false;
     },3000)
   });
 }
}
