import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { generatePaginationHeader, getPaginatedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessagingService {

  constructor(private http: HttpClient) { }
  baseUrl = environment.apiUrl;
  getMessages(container:string, pageNumber: number, pageSize: number ){
    var params = generatePaginationHeader(pageNumber, pageSize);
    params = params.append("Container", container);
    return getPaginatedResult<Message[]>(this.baseUrl +'message', params, this.http);
  }

  getMessageThread(username){
    return this.http.get<Message[]>(this.baseUrl + 'message/thread/'+username);
  }

  sendMessage(username: string, content: string){
    return this.http.post<Message>(this.baseUrl + 'message' ,{recipientUserName: username, content: content} );
  }

  deleteMessage(id){
    return this.http.delete(this.baseUrl+'message/'+id);
  }
}
