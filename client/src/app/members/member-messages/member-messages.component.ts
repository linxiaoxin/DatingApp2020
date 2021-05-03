import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessagingService } from 'src/app/_services/messaging.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit, OnDestroy {

  constructor(public messagingService: MessagingService) { }
  
  //@Input() messages: Message[];
  @ViewChild('sendMsgFrom') sendMsgForm: NgForm;
  @Input() senderName: string;
  newMsg: string;

  ngOnInit(): void {
  }
  ngOnDestroy(): void {
    this.messagingService.destroySignalRConnection();
  }
  
  sendMessage(){
    this.messagingService.sendMessage(this.senderName, this.newMsg)
    .then( ()=>{
      this.newMsg ="";
     })
    .catch(error => console.log(error));
    //this.sendMsgForm.reset();
    }

}
