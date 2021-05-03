import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { throwIfEmpty } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user'
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  constructor(private accountService: AccountService, private router: Router, private toastr: ToastrService) { }
  currentUser : Observable<User>;
  ngOnInit(): void {
    //this.initialised();
    this.currentUser = this.accountService.currentUser$;
  }
  model: any = {};
  login()
  {
    this.accountService.login(this.model).subscribe(response =>{
      console.log(response);
      this.router.navigateByUrl('/members');
    }, error =>{
      console.log(error);
    });
  }

  logout()
  {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
  // user async pipe instead as subsription without unsubsribe result in memory leak
  /*initialised()
  {
    this.accountService.currentUser$.subscribe(user => {
      this.loggedIn = !!user;
      console.log(user);
    }, error =>{
      console.log(error);
    })
  }*/
}
