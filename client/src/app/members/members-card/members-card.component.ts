import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-members-card',
  templateUrl: './members-card.component.html',
  styleUrls: ['./members-card.component.css']
})
export class MembersCardComponent implements OnInit {
  @Input() member: Member;
  constructor(private memberService: MembersService, private toastrService: ToastrService) { 
  }

  ngOnInit(): void {
  }
  addLike(member: Member){
    this.memberService.addLikes(member.userName).subscribe(() => {
      this.toastrService.success("You have successfully liked "+member.knownAs);
    })
  }
}
