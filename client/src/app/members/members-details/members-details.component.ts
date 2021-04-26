import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessagingService } from 'src/app/_services/messaging.service';

@Component({
  selector: 'app-members-details',
  templateUrl: './members-details.component.html',
  styleUrls: ['./members-details.component.css']
})
export class MembersDetailsComponent implements OnInit {
  @ViewChild('memberTabSet', {'static' : true}) memberTabSet: TabsetComponent; 
  member: Member;
  constructor(private memberService: MembersService, private messagingService: MessagingService, private router: ActivatedRoute) { }
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  messages: Message[] = [];

  ngOnInit(): void {

    this.router.data.subscribe(data =>{
      this.member = data.member;
    })
    this.router.queryParams.subscribe(params => {
      if(params.tab) this.selectTab(params.tab);
      //console.log('query params tabs:' + params.tab);
    });
    this.galleryOptions = [{
      width: '500px',
      height: '500px',
      imagePercent: 100,
      thumbnailsColumns: 4,
      imageAnimation: NgxGalleryAnimation.Slide,
      preview: false
    }];
    this.galleryImages = this.getImages();

  }

  getImages(): NgxGalleryImage[]{
    const imageurl = []
    for(const photo of this.member.photos){
      imageurl.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      });
    }
    return imageurl;
  }
  loadMember(){
    this.memberService.getMember(this.router.snapshot.paramMap.get('username') ).subscribe(member =>{
        this.member = member;
        this.galleryImages = this.getImages();
    });
  }
  
  loadMessage(){
    this.messagingService.getMessageThread(this.member.userName).subscribe(response => {
      this.messages = response;
    });
  }

  loadTab(event: TabDirective){
    if(event.heading === 'Messages' && this.messages.length ==0 )
    { 
      this.messagingService.createSignalRConnection(this.member.userName);
    }
    else 
    this.messagingService.destroySignalRConnection();
  }

  selectTab(heading){
    this.memberTabSet.tabs.forEach(tab =>{
      if(tab.heading ===heading) tab.active = true;
    })
  }
}
