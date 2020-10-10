import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
@Output() cancelRegisteration = new EventEmitter();

  constructor(private accountService: AccountService, private toastr: ToastrService) { }
  @Input() UsersFromHomeComponent
  model: any ={};

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe(response =>{
      console.log(response)}, error=>{
        console.log(error);
        this.toastr.error(error.error);
      }
    );
    this.cancel();
  }

  cancel(){
    this.cancelRegisteration.emit(false);
  }
}
