import { BehaviorSubject } from 'rxjs';
import { ConfigService } from './config.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {DomSanitizer} from '@angular/platform-browser';
import { ChatContent, ChatService } from './chat.service';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {

  public imageUrl:string=null;

  private mesPhotoSub=new BehaviorSubject<string>(null);

  public mesPhoto= this.mesPhotoSub.asObservable();

  constructor(private http:HttpClient,private config:ConfigService,private sanitizer:DomSanitizer) { }

  async UploadPhoto(photo){
    let url = await this.config.getConfig("updatephoto");

    const uploadData = new FormData();
    uploadData.append(photo.name, photo, photo.name);

    return this.http.put(url,uploadData);
  }

  async UploadGroupPhoto(photo,chatId:number){
    let url = await this.config.getConfig("updateGroupphoto");

    const uploadData = new FormData();
    uploadData.append(photo.name, photo, photo.name);

    return this.http.post(`${url}?chatId=${chatId}`,uploadData);
  }

  async UploadMessagePhoto(photo){
    let url = await this.config.getConfig("uploadmessagephoto");

    const uploadData = new FormData();
    uploadData.append(photo.name, photo, photo.name);


    return this.http.put<string>(url,uploadData,{responseType:'text' as 'json'}).subscribe((res)=>this.UpdateMesPhoto(res));
  }

  public UpdateMesPhoto(photo:string){
    this.mesPhotoSub.next(photo);
  }
}
