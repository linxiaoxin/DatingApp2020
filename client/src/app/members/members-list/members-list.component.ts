import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.css']
})
export class MembersListComponent implements OnInit {
  members: Member[];
  constructor(private membersvc: MembersService) { }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    this.membersvc.getMembers().subscribe(member => {this.members = member})
  }

}
