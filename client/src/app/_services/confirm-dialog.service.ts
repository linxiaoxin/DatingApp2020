import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../_dialog/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
  modalRef: BsModalRef
  constructor(private modalService: BsModalService) { }

  OpenConfirmDialog(title:string='Confirmation', message: string= 'Are you sure you want to do this?'
  , btnOKText:string="OK", btnCancelText: string="Cancel"): Observable<boolean>{
    const config = {
      initialState: {
        title,
        message,
        btnOKText,
        btnCancelText
      }
    };
    this.modalRef = this.modalService.show(ConfirmDialogComponent, config);

    return new Observable(this.getResult());
  }

  private getResult(){
    return (observer)=>{
      const subscription =this.modalRef.onHidden.subscribe(()=>{
        observer.next(this.modalRef.content.result);
        observer.complete();
      });
      return {
        unsubscribe(){
            subscription.unsubscribe();
        }
      }
    }
  }

}
