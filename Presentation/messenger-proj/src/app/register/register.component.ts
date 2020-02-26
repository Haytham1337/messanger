import { RegisterGuard } from './../register.guard';
import { AuthService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  isnotvalid:boolean;
  userdata={email:'',password:'',passwordconfirm:''};
  constructor(private auth:AuthService,private router:Router,private guard: RegisterGuard) { }

  ngOnInit() {
    this.auth.errorOccured=false;
  }

  async register(){
     if(this.userdata.password==this.userdata.passwordconfirm&&this.isPossiblyValidEmail(this.userdata.email))
     {
        let res=await this.auth.fillRegister(this.userdata);

        this.auth.errorOccured=!res;

        if(res){
          this.guard.isenabled=true;
          this.router.navigate(['/fillinfo']);
        }
      }

      this.isnotvalid=true;

}

   isPossiblyValidEmail(txt) {
    return txt.length > 5 && txt.indexOf('@')>0;
 }

}
