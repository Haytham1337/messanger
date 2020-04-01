import { RegisterGuard } from './../register.guard';
import { AuthenticationService } from './../services/auth.service';
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
  constructor(public auth:AuthenticationService,public router:Router,public guard: RegisterGuard) { }

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

      this.auth.errorOccured=true;
}

   isPossiblyValidEmail(txt) {
    return txt.length > 5 && txt.indexOf('@')>0;
 }

}
