import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators'
import { User } from '../_models/user';

//services is injectable and a singleton
@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient) { }

  baseUrl = 'https://localhost:5001/api/';
  private currentUserReplay = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserReplay.asObservable(); //$ to indicate that variable is observables

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        if (response) {
          localStorage.setItem('user', JSON.stringify(response));
          this.currentUserReplay.next(response);
        }
        return response;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserReplay.next(null);
  }

  setCurrentUser(user: User) {
    this.currentUserReplay.next(user);
  };

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        if(response){
          localStorage.setItem('user', JSON.stringify(response));
          this.currentUserReplay.next(response);
        }
        return response;
      })
    );
  }
}
