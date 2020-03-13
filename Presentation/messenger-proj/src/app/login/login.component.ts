import { UserService } from './../services/user.service';
import { Router } from '@angular/router';
import { ConfigService } from './../services/config.service';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { AuthService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';
declare var FB: any;


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  public isnotvalid=false;
  userdata={email:'',password:''};

  authResp:any;

  constructor(private auth:AuthService,private route:Router,private user:UserService) { }

  ngOnInit() {
    this.auth.errorOccured=false;

    (window as any).fbAsyncInit = function() {
      FB.init({
        appId      : '2313754038918109',
        cookie     : true,
        xfbml      : true,
        version    : 'v6.0'
        });
      FB.AppEvents.logPageView();
    };

    (function(d, s, id){
       var js, fjs = d.getElementsByTagName(s)[0];
       if (d.getElementById(id)) {return;}
       js = d.createElement(s); js.id = id;
       js.src = "https://connect.facebook.net/en_US/sdk.js";
       fjs.parentNode.insertBefore(js, fjs);
     }(document, 'script', 'facebook-jssdk'));

     FB.getLoginStatus(function(response) {
      console.log(response);
  });

  }

  async signin(){
    await this.auth.signin(this.userdata)
    .then(()=>{
      this.isnotvalid=true;
    });
  }

  submitLogin(){
    FB.login(async (response)=>
        {
          if (response.authResponse)
          {
            this.authResp=response;
            console.log(response);                
          }
      },{ auth_type: 'reauthenticate',scope:'email'});

  }

  signout(){
    let auth=this.authResp;
    FB.logout(function(auth){});
  }

}
