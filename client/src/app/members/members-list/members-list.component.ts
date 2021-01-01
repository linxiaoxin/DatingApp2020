import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/usersParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.css']
})
export class MembersListComponent implements OnInit {
  members : Member[];
  pagination: Pagination ;
  userParams: UserParams;
  genderList= [{value: 'male', display: 'Male'},
               {value: 'female', display: 'Female'}] ;

  constructor(private membersvc: MembersService) { 
    this.userParams = this.membersvc.getUserParams();
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    this.membersvc.getMembers(this.userParams).subscribe(response =>{
      this.members= response.result;
      this.pagination = response.pagination;
    }
    ); 
  }
  pageChange(event: any){
    this.userParams.pageNumber = event.page;
    this.membersvc.setUserParam(this.userParams);
    this.loadMember();
  }
  resetParams(){
    this.userParams = this.membersvc.resetUserParam();
  }
}
