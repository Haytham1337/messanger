import { PhotoService } from './../services/photo.service';
import { AuthenticationService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import {UserService, User} from './../services/user.service';


@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  public currentUser:User=new User();
  constructor(public auth:AuthenticationService,public userservice:UserService,public photoser:PhotoService) { }

  ngOnInit() {
    this.userservice.data.subscribe(user=>this.currentUser=user);
  }

  async fileselected(event){
   (await this.photoser.UploadPhoto(event.target.files[0]))
        .subscribe(res=>this.userservice.SetCurrentUser())
  }
}