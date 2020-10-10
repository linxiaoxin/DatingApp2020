import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorsInterceptor implements HttpInterceptor {

  constructor(private toastr: ToastrService, private router: Router) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        switch (error.status) {
          case 500: //server error
            const navigateExtras: NavigationExtras = { state: { error: error.error } };
            this.router.navigateByUrl('/server-error', navigateExtras);
            break;
          case 401: //Unauthorised
            this.toastr.error(error.statusText, error.status);
            break;
          case 404://not found
            this.router.navigateByUrl('/not-found');
            break;
          case 400://bad request
            if (error.error.errors) {
              const modalStateErrors = [];
              for (let i in error.error.errors) {
                modalStateErrors.push(error.error.errors[i]);
              }
              throw modalStateErrors.flat();
            }
            else {
              this.toastr.error(error.error, error.status);
            }
            break;
          default:
            this.toastr.error("Something out of exception occurs.", error.status);
            console.log(error);
            break;
        }
        return throwError(error);
      })
    );
  }
}
