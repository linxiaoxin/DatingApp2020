import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {

  constructor(private Http: HttpClient) { }
  baseUrl = 'https://localhost:5001/api/'
  validationErrors : string[] = [];

  ngOnInit(): void {
  }

  test500error() {
    this.Http.get(this.baseUrl + 'buggy/server-error').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }
  test401error() {
    this.Http.get(this.baseUrl + 'buggy/Auth').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  test404error() {
    this.Http.get(this.baseUrl + 'buggy/not-found').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  test400error() {
    this.Http.get(this.baseUrl + 'buggy/bad-request').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }
  testValidationerror() {
    this.Http.post(this.baseUrl + 'account/register', {}).subscribe(response => {
      console.log(response);
    }, error => {
      this.validationErrors = error;
      console.log(error);
    })
  }

}
