import { UserService } from './../services/user.service';
import { Router } from '@angular/router';
import { ConfigService } from './../services/config.service';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { AuthenticationService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  public isnotvalid=false;
  userdata={email:'',password:''};

  authResp:any;

  constructor(public auth:AuthenticationService,public route:Router,public user:UserService,public http:HttpClient) { }

  ngOnInit() {
    this.auth.errorOccured=false;
  }

  async signin(){
    await this.auth.signin(this.userdata)
    .then(()=>{
      this.isnotvalid=true;
    });
  }
}
