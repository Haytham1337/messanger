import { SignInResponce } from './auth.service';
import { HttpInterceptor } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { ConfigService } from './config.service';
import { Router } from '@angular/router';


@Injectable({
  providedIn: 'root'
})
export class RefreshTokeninterceptorService implements HttpInterceptor{

constructor(private client:HttpClient,private config:ConfigService,private router:Router) { }

 intercept(req,next){

  if(localStorage["expiresIn"]==undefined||localStorage["expiresIn"]=="")
  {
    return next.handle(req.clone());
  }

  if(Date.parse(localStorage["expiresIn"])<Date.now()){
    localStorage.setItem('expiresIn',"");
    let url= `https://localhost:44334/api/Auth/ExchangeTokens`;

    var data={
      AccessToken:localStorage["token"],
      RefreshToken:localStorage["refreshToken"]
    }

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    this.client.post<SignInResponce>(url,JSON.stringify(data),{headers:headers}).subscribe(res=>{
      localStorage.setItem('token',res.access_Token);
      localStorage.setItem('expiresIn',res.expiresIn.toString());
      localStorage.setItem('refreshToken',res.refresh_Token);
    },err=>console.log(err + "happened"));
  }

  return next.handle(req.clone());
}

}
