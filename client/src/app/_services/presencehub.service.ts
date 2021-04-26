import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresencehubService {
  signalrHubsUrl = environment.signlrHubsUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createSignalrConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
        .withUrl(this.signalrHubsUrl+"presence", {accessTokenFactory: ()=> user.token})
        .build(); 
    
    this.hubConnection.on("UserIsOnline", username=>{
      this.onlineUser$.pipe(take(1)).subscribe(usernames =>{
        this.onlineUserSource.next([...usernames, username]);
      })
    } );

    this.hubConnection.on("UserIsOffline", username=>{
      this.onlineUserSource.pipe(take(1)).subscribe(usernames =>{
        this.onlineUserSource.next([...usernames.filter(x => x != username)])
      })
    } );

    this.hubConnection.on("GetOnlineUsers", (users: string[]) => {
      this.onlineUserSource.next(users);
    });

    this.hubConnection.on("NotifyNewMsg", ({username, knownAs}) =>{
      this.toastr.info(knownAs +' has sent you a message.')
      .onTap
      .pipe(take(1))
      .subscribe(() => {
        this.router.navigateByUrl('/members/'+username+'?tab=Messages')
      })
    });
    this.hubConnection.start().catch(error => {this.toastr.error(error)});
  }

  destroySignalRConnection(){
    this.hubConnection.stop();
  }
}
