import { PhotoService } from './services/photo.service';
import { UserService } from './services/user.service';
import { CurrentDate } from './pipes/date.pipe';
import { RegisterGuard } from './register.guard';
import { TokeninterceptorService } from './services/tokeninterceptor.service';
import { RefreshTokeninterceptorService } from './services/RefreshTokenInrceptor.service';

import { AuthGuard } from './auth.guard';
import { ConfigService } from './services/config.service';
import { AuthService } from './services/auth.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ChatComponent } from './chat/chat.component';
import { RouterModule, Routes } from '@angular/router';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CookieService } from 'node_modules/ngx-cookie-service';
import { ProfileComponent } from './profile/profile.component';
import { FilluserinfoComponent } from './filluserinfo/filluserinfo.component';
import { ChatlistComponent } from './chatlist/chatlist.component';
import { SearchComponent } from './search/search.component';
import { FriendinfoComponent } from './friendinfo/friendinfo.component';
import { ChannelCreateComponent } from './channel-create/channel-create.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { GroupinfoComponent } from './groupinfo/groupinfo.component';
import { DeleteCheckComponent } from './delete-check/delete-check.component';
import { DatePipe } from '@angular/common';


const appRoutes: Routes = [
   { path: '', redirectTo:'/chat',pathMatch:'full' },
   { path: 'chat', component:ChatComponent,canActivate:[AuthGuard]},
   { path: 'profile', component:ProfileComponent,canActivate:[AuthGuard]},
   { path: 'signin', component:LoginComponent },
   { path: 'check', component:DeleteCheckComponent },
   { path: 'friendinfo', component:FriendinfoComponent },
   { path: 'groupinfo', component:GroupinfoComponent },
   { path: 'register', component:RegisterComponent },
   { path: 'fillinfo', component:FilluserinfoComponent ,canActivate:[RegisterGuard]},
   { path: 'createchannel', component:ChannelCreateComponent ,canActivate:[RegisterGuard]}
]



@NgModule({
   declarations: [
      AppComponent,
      NavbarComponent,
      LoginComponent,
      RegisterComponent,
      ChatComponent,
      ProfileComponent,
      FilluserinfoComponent,
      CurrentDate,
      ChatlistComponent,
      SearchComponent,
      FriendinfoComponent,
      ChannelCreateComponent,
      GroupinfoComponent,
      DeleteCheckComponent
   ],
   imports: [
      RouterModule.forRoot(appRoutes),
      BrowserModule,
      HttpClientModule,
      AppRoutingModule,
      FormsModule,
      NgMultiSelectDropDownModule.forRoot()
   ],
   providers: [
      CookieService,
      AuthGuard,
      UserService,
      DatePipe,
      CurrentDate,
      PhotoService,
      RegisterGuard,
      AuthService,
      ConfigService,
      {
         provide: HTTP_INTERCEPTORS,
         useClass: TokeninterceptorService,
         multi: true
       },
       {
         provide: HTTP_INTERCEPTORS,
         useClass: RefreshTokeninterceptorService,
         multi: true
       }
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
