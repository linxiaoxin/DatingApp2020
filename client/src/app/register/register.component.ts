import { Route } from '@angular/compiler/src/core';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
@Output() cancelRegisteration = new EventEmitter();
maxDate : Date;

  constructor(private accountService: AccountService, private toastr: ToastrService, private fb: FormBuilder
    , private router: Router) { }
  @Input() UsersFromHomeComponent
  registerUserForm: FormGroup
  validationErrors: string [] = [];


  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear( this.maxDate.getFullYear() -18);
  }

  initializeForm(){
    this.registerUserForm = this.fb.group({
        gender:['Male',Validators.required],
        username : ['',Validators.required],
        knownAs : ['',Validators.required],
        dateOfBirth : ['',Validators.required],
        city : ['',Validators.required],
        country : ['',Validators.required],
        password: ['',[Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
        confirmPassword: ['', [Validators.required,this.isMatchValidator('password')]]
    });
  }

  isMatchValidator(matchToControlName: string): ValidatorFn {
    return(control: AbstractControl): {[key: string]: any} | null => {
        return control?.parent?.controls[matchToControlName].value === control?.value? null: {isMatching: control?.value};
    }
  }
  register(){
    //console.log(this.registerUserForm.value);
     this.accountService.register(this.registerUserForm.value).subscribe(response =>
      {
        this.router.navigateByUrl('/members');
      }, error=>{
        this.validationErrors = error;
        //console.log(error);
         //this.toastr.error(error.error);
       }
     );
     //this.cancel();
  }

  cancel(){
    this.cancelRegisteration.emit(false);
  }
}
