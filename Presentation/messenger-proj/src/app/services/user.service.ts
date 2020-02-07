import { PhotoService } from './photo.service';
import { ConfigService } from './config.service';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import {BehaviorSubject} from 'rxjs';

export class User{
  id:number;
  photoName:string;
  nickname:string;
  phone:string;
  email:string;
  age:number;
}

@Injectable({
  providedIn: 'root'
})
export class UserService  {

  private currentUser=new BehaviorSubject<User>(new User());
  data=this.currentUser.asObservable();

  private searchUsers=new BehaviorSubject<User[]>([]);
  searchdata=this.searchUsers.asObservable();

  constructor(private http:HttpClient,private config:ConfigService,private photoservice:PhotoService) { }

  public async UpdateUser(data) {
    let url=await this.config.getConfig("updateuser");

    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');
            
    return this.http.post(url,JSON.stringify(data),{headers:headers}).toPromise();
  }

  public async SetCurrentUser(){
    let url=await this.config.getConfig("getuserinfo");
    
    let headers = new HttpHeaders();
    headers= headers.append('content-type', 'application/json');

    await this.photoservice.GetPhoto();

    return await this.http.get<User>(url,{headers:headers}).toPromise()
    .then( res=>
      {res.photoName=this.photoservice.imageUrl; 
      this.updateCurrentUser(res);});
  }

  updateCurrentUser(user:User){
    this.currentUser.next(user);
  }

  updateSearchUsers(values:User[]){
    this.searchUsers.next(values);
  }

  public async SearchUsers(filter:string){
    let url =await this.config.getConfig("search")+`?Filter=${filter}`;
    let imgpath=await this.config.getConfig("photopath");
    console.log(imgpath);
    return await this.http.get<User[]>(url).toPromise()
    .then(res=>
      {
       let mappedres= res.map(user=>{
          user.photoName=`${imgpath}/${user.photoName}`;
          return user;
        })
        console.log(mappedres);
        this.updateSearchUsers(mappedres);
        return mappedres;
      });
  }
}
