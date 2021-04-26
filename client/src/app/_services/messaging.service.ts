import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Console } from 'console';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { MsgGroup } from '../_models/msgGroup';
import { User } from '../_models/user';
import { AccountService } from './account.service';
import { generatePaginationHeader, getPaginatedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessagingService {

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
   }
  baseUrl = environment.apiUrl;
  signalRUrl = environment.signlrHubsUrl;
  private hubConnection : HubConnection;
  user: User;
  private messagesSource = new BehaviorSubject<Message[]>([]);
  message$ = this.messagesSource.asObservable();

  createSignalRConnection(otherusername){
    this.hubConnection = new HubConnectionBuilder().withUrl(this.signalRUrl+'messaging?user='+otherusername, {accessTokenFactory: ()=> this.user.token})
      .withAutomaticReconnect()
      .build();
    
      //this.getMessageThread(otherusername).subscribe(messages => 
      //  this.messagesSource.next(messages)
      //  );

      this.hubConnection.on("AllMessageThread", message => {
          this.messagesSource.next(message);
      })
      this.hubConnection.on("NewMessage", message => {
        this.message$.pipe(take(1)).subscribe(messages => 
            this.messagesSource.next([...messages, message])
          )
      })
      this.hubConnection.on("UpdatedGroup", (group: MsgGroup) => {
        this.message$.pipe(take(2)).subscribe(messages => {
          if(group.connections.some(x=> x.username ==otherusername)){
            messages.forEach(message => {
              if(!message.dateRead) message.dateRead = new Date(Date.now());
            })
            this.messagesSource.next([...messages]);
          }
        });
      });

      this.hubConnection.start();
  }
  destroySignalRConnection(){
    if(this.hubConnection)
      this.hubConnection.stop()
  }

  getMessages(container:string, pageNumber: number, pageSize: number ){
    var params = generatePaginationHeader(pageNumber, pageSize);
    params = params.append("Container", container);
    return getPaginatedResult<Message[]>(this.baseUrl +'message', params, this.http);
  }

  getMessageThread(username){
    return this.http.get<Message[]>(this.baseUrl + 'message/thread/'+username);
  }

  sendMessage(username: string, content: string){
    //return this.http.post<Message>(this.baseUrl + 'message' ,{recipientUserName: username, content: content} );
    return this.hubConnection.invoke("AddMessage", {recipientUserName: username, content: content});
  }

  deleteMessage(id){
    return this.http.delete(this.baseUrl+'message/'+id);
  }
}
