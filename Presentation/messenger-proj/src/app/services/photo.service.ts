import { BehaviorSubject } from 'rxjs';
import { ConfigService } from './config.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {DomSanitizer} from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {

  public imageUrl:string=null;

  constructor(private http:HttpClient,private config:ConfigService,private sanitizer:DomSanitizer) { }

  async UploadPhoto(photo){
    let url = await this.config.getConfig("updatephoto");

    const uploadData = new FormData();
    uploadData.append(photo.name, photo, photo.name);

    return this.http.post(url,uploadData);
  }
}
