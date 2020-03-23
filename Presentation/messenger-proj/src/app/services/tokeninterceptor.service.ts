import { AuthenticationService } from './auth.service';
import { HttpInterceptor } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokeninterceptorService implements HttpInterceptor{

constructor(private auth:AuthenticationService) { }

intercept(req,next){
  let tokenizedreq=req.clone({
    setHeaders: {
      Authorization: `Bearer ${localStorage["token"]}`
  }
  })
  return next.handle(tokenizedreq);
}

}
