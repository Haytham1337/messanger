import { UserService } from './user.service';
import { ConfigService } from './config.service';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'angularx-social-login';
import { FacebookLoginProvider, GoogleLoginProvider } from 'angularx-social-login';

export class UserInfo{
  Email:string;
  Password:string;
  PasswordConfirm:string;
  NickName:string;
  Sex:number;
  PhoneNumber:string;
  Age:number;
}

export class SignInResponce{
  access_Token:string;
  expiresIn:Date;
  refresh_Token:string;
}

export class FbUser{
email:string;
lastName:string;
photoUrl:string;
gender:string;
accessToken:string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

   userInfo:UserInfo=new UserInfo();

   errorOccured:boolean=false;

   errorMessage:string;

    constructor(private http:HttpClient,private config:ConfigService,private router:Router,private userservice:UserService,
      private authService:AuthService) {}

    async register(){
      let url=await this.config.getConfig("signup");

      let headers = new HttpHeaders();
      headers= headers.append('content-type', 'application/json');
      
      return this.http.post(url,JSON.stringify(this.userInfo),{responseType:'text',headers:headers})
          .subscribe(
            async res=>{
              await this.router.navigate(['/signin']);
            },err=>this.errorOccured=true);
    }

    async signin(user){
      let url=await this.config.getConfig("signin");
      
      let headers = new HttpHeaders();
      headers= headers.append('content-type', 'application/json');
      
      return await this.http.post<SignInResponce>(url,JSON.stringify(user),{headers:headers})
      .subscribe(
        async res=>{
           localStorage.setItem('token',res.access_Token);
           localStorage.setItem('expiresIn',res.expiresIn.toString());
           localStorage.setItem('refreshToken',res.refresh_Token);
           await this.userservice.SetCurrentUser();
           await this.router.navigate(['/chat']);
        },err=>{
          this.errorOccured=true;
          if(typeof err.error==="string"){
            this.errorMessage=err.error;
          }
          else{
            this.errorMessage="Your data is not valid!!";
          }
        });
    }

    async fillRegister(data){
      let url=await this.config.getConfig("checkemail");
      this.userInfo.Email=data.email;
      this.userInfo.Password=data.password;
      this.userInfo.PasswordConfirm=data.passwordconfirm;

      let headers = new HttpHeaders();
      headers= headers.append('content-type', 'application/json');
      
      return this.http.post<boolean>(url,this.userInfo,{headers:headers}).toPromise();
    }

    fillInfo(data){
      this.userInfo.NickName=data.NickName;
      this.userInfo.Sex= parseInt(data.Sex);
      this.userInfo.PhoneNumber=data.PhoneNumber;
      this.userInfo.Age=data.Age;
      this.register();
    }
    
    signout(){
      this.router.navigate(['/signin']);
      localStorage.clear();
    }

    looggedIn(){
      return !!localStorage.getItem('token');
    }

    gettoken(){
      return localStorage.getItem('token');
    }

    logInWithFacebook(platform: string): void {
      platform = FacebookLoginProvider.PROVIDER_ID;

      this.authService.signIn(platform).then(
      async (response) => {

           let fbuser=new FbUser();
           fbuser.email=response.email;
           fbuser.lastName=response.lastName;
           fbuser.photoUrl=response.photoUrl;
           fbuser.gender="Male";
           fbuser.accessToken=response.authToken

            let url=await this.config.getConfig("signinFB");

            let headers = new HttpHeaders();
            headers= headers.append('content-type', 'application/json');
            
            return this.http.post<SignInResponce>(url,JSON.stringify(fbuser),{headers:headers})
                .subscribe(
                  async res=>{
                    localStorage.setItem('token',res.access_Token);
                    localStorage.setItem('expiresIn',res.expiresIn.toString());
                    localStorage.setItem('refreshToken',res.refresh_Token);
                    await this.userservice.SetCurrentUser();
                    await this.router.navigate(['/chat']);
                  },err=>this.errorOccured=true);
            });
    }
}
