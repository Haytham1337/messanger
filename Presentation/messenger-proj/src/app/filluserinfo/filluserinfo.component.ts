import { AuthenticationService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-filluserinfo',
  templateUrl: './filluserinfo.component.html',
  styleUrls: ['./filluserinfo.component.css']
})
export class FilluserinfoComponent implements OnInit {


  userdata={NickName:'',Sex:0,PhoneNumber:'',Age:null};
  isnotvalid:boolean;
  constructor(private auth:AuthenticationService) { }

  ngOnInit() {
  }

  register(){
    if(this.userdata.Age!=null&&this.userdata.NickName.length>0){
      this.isnotvalid=false;
      this.auth.fillInfo(this.userdata);
    }
    else{
      this.isnotvalid=true;
    }
  }

  validate(event){
    if(!isFinite(event.key)){
      this.userdata.PhoneNumber=this.userdata.PhoneNumber.slice(0,-1);
    }    
  }
}
