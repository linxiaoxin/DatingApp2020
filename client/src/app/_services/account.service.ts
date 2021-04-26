import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresencehubService } from './presencehub.service';

//services is injectable and a singleton
@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient, private presenceHub: PresencehubService) { }

  baseUrl = environment.apiUrl;
  private currentUserReplay = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserReplay.asObservable(); //$ to indicate that variable is observables

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        if (response) {
          this.setCurrentUser(response);
          this.presenceHub.createSignalrConnection(response);          
        }
        return response;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserReplay.next(null);
    this.presenceHub.destroySignalRConnection();
  }

  setCurrentUser(user: User) {
    const roles = this.decodedToken(user.token).role;
    console.log(typeof(roles));
    Array.isArray(roles)? user.roles = roles: user.roles = [roles];
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserReplay.next(user);
  };

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        if(response){
          this.setCurrentUser(response);
          this.presenceHub.createSignalrConnection(response);
        }
        return response;
      })
    );
  }

  decodedToken(token){
    console.log(token);
    return JSON.parse(atob(token.split('.')[1]));
  }
}
