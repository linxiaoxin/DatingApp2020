import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  constructor(private memberService: MembersService) { }
  predicate: string = 'like';
  likes: Partial<Member[]>;
  pagination: Pagination ;
  pageNumber: number = 1;
  pageSize: number = 5;
  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.memberService.getLikes(this.predicate,this.pageNumber, this.pageSize ).subscribe(data =>{
      this.likes = data.result;
      this.pagination = data.pagination;
    });
  }
  pageChange(event: any){
    this.pageNumber = event.page;
    this.loadLikes();
  }
}
