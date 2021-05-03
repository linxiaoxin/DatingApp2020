import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { ConfirmDialogService } from '../_services/confirm-dialog.service';
import { MessagingService } from '../_services/messaging.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  constructor(private messagingService: MessagingService, private confirmService: ConfirmDialogService) { }
  container: string = 'Unread';
  pageNumber: number =1;
  pageSize: number = 10;
  pagination: Pagination;
  messages: Message[];
  loading;

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading=true;
    this.messagingService.getMessages(this.container, this.pageNumber, this.pageSize).subscribe(response => {
      this.pagination = response.pagination;
      this.messages = response.result;
      this.loading=false;
    });
  }

  pageChange(event: any){
    this.pageNumber = event.page;
    this.loadMessages();
  }
  deleteMessage(id){
    this.confirmService.OpenConfirmDialog("Delete Photo", "Action cannot be undone. Ok to proceed", "OK", "Cancel").subscribe(result =>{
      if(result){
        this.messagingService.deleteMessage(id).subscribe(() =>{
          this.messages.forEach((m, index) => {
            if(m.id == id)  this.messages.splice(index, 1); 
          })
        });
      }
    });
  }
}
