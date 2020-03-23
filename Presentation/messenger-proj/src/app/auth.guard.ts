import { AuthenticationService } from './services/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private auth:AuthenticationService,private route:Router){

  }
  canActivate():boolean{
    if(this.auth.looggedIn()){
      return true;
    }
    else{
      this.route.navigate(['/signin'])
      return false;
    }
  }
  
}
