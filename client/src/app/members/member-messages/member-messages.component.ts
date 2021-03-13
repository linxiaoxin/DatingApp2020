import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessagingService } from 'src/app/_services/messaging.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  constructor(private messagingService: MessagingService) { }
  @Input() messages: Message[];
  @ViewChild('sendMsgFrom') sendMsgForm: NgForm;
  @Input() senderName: string;
  newMsg: string;

  ngOnInit(): void {
  }

  sendMessage(){
    this.messagingService.sendMessage(this.senderName, this.newMsg).subscribe(data =>{
      this.messages.push(data);
      this.sendMsgForm.reset();
    });
  }

}
