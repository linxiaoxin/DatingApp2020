import { Component, OnInit } from '@angular/core';
import { from } from 'rxjs';
import { AccountService } from './_services/account.service';
import { User } from './_models/user'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Let\'s Meet Up';
  users: any;

  constructor(private accountService: AccountService){

  }
  ngOnInit(){
    //this.getusers();
    this.setCurrentUser();
  }
  /*
  getusers(){
    this.http.get('https://localhost:5001/api/users').subscribe(response =>{
      this.users=response;
    }, error => {
      console.log(error);
    });
  }*/

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    this.accountService.setCurrentUser(user);
  };
}


