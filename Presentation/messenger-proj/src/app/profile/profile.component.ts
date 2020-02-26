import { PhotoService } from './../services/photo.service';
import { Component, OnInit } from '@angular/core';
import { UserService, User } from '../services/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  currentUser:User=new User();

  constructor(private photo:PhotoService,private userservice:UserService) { }

 ngOnInit() {
    this.userservice.data.subscribe(user=>this.currentUser=user);
    this.userservice.valid=false;
  }

  UpdateUser(){
    console.log(this.currentUser);
    this.userservice.updated=false;
    if(this.currentUser.age>100 || this.currentUser.age<0 || this.currentUser.nickName==""){
      this.userservice.valid=true;
      this.userservice.SetCurrentUser();
    }
    else{
      this.userservice.valid=false;
      this.userservice.UpdateUser(this.currentUser);
    }
  }

  validate(event){
    if(!isFinite(event.key)){
      this.currentUser.phone=this.currentUser.phone.slice(0,-1);
    }    
  }
}
