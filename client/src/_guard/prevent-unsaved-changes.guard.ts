import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from 'src/app/members/member-edit/member-edit.component';
import { ConfirmDialogService } from 'src/app/_services/confirm-dialog.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmService: ConfirmDialogService){}

  canDeactivate(
    component: MemberEditComponent): Observable<boolean> | boolean  {
      if(component.editForm.dirty)  
        return this.confirmService.OpenConfirmDialog('Confirmation','Are you sure you want to continue? Any unsaved changes will be lost.', 'OK', 'Cancel');
      return true;
  }
  
}
