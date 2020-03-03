import { ChatService, Group } from './../services/chat.service';
import { UserService, User } from './../services/user.service';
import { Component, OnInit } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-channel-create',
  templateUrl: './channel-create.component.html',
  styleUrls: ['./channel-create.component.css']
})
export class ChannelCreateComponent implements OnInit {

  SearchUsers:User[];
  selectedItems :User [];
  dropdownSettings:IDropdownSettings = {};
 
  group:Group=new Group();

  constructor(private userservice:UserService,private chatservice:ChatService) { }

  ngOnInit() {

    this.userservice.searchdata.subscribe((res)=>this.SearchUsers=res);

    this.dropdownSettings= {
      singleSelection: false,
      idField: 'id',
      textField: 'email',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 3,
      allowSearchFilter: true,
      enableCheckAll:false,
      defaultOpen:true,
      allowRemoteDataSearch:true,
      clearSearchFilter:true
    };

    this.group.UsersId=[];

  }

  onItemSelect(item: User) {
     this.group.UsersId.push(item.id);
  }

  onFilterChange(filter:string) {
    this.userservice.SearchUsers(filter);
  }

  createGroup(){
    this.chatservice.CreateGroup(this.group).then(()=>{
      this.selectedItems=[];
      this.group.IsChannel=false;
      this.group.GroupName="";
    });

  }
}
