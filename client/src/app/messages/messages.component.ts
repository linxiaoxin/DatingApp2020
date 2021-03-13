import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { MessagingService } from '../_services/messaging.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  constructor(private messagingService: MessagingService) { }
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
    this.messagingService.deleteMessage(id).subscribe(() =>{
      this.messages.forEach((m, index) => {
        if(m.id == id)  this.messages.splice(index, 1); 
      })
    });
  }
}
