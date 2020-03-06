import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'MessageDate'
})
export class CurrentDate implements PipeTransform {

  transform(value: Date): string {
    let messageTime=new Date(value);

    let minutes=messageTime.getMinutes()<10?`0${messageTime.getMinutes()}`:messageTime.getMinutes();
    
    return `${messageTime.getHours()} :${minutes}`
  }
}
