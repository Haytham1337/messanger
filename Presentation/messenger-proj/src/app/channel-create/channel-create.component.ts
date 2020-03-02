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
  UsersId:number[]=[];

  constructor(private userservice:UserService) { }

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
    
  }

  onItemSelect(item: User) {
     this.UsersId.push(item.id);
  }

  onFilterChange(filter:string) {
    this.userservice.SearchUsers(filter);
  }

}
