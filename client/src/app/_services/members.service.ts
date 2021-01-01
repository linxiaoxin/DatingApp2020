import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { MapOperator } from 'rxjs/internal/operators/map';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PgainationResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/usersParams';
import { AccountService } from './account.service';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  user: User;
  userParams: UserParams;
  members : Member[] = [];
  memberCache= new Map();

  constructor(private http: HttpClient, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.userParams = new UserParams(user);
      this.user = user;
    })
  }

  getUserParams()
  {
    return this.userParams
  }
  setUserParam(userParam: UserParams)
  {
    this.userParams = userParam;
  }
  resetUserParam(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }
  getMembers(userParams: UserParams){
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return of(response);

    var params = this.generatePaginationHeader(userParams.pageNumber, userParams.pageSize);
    params = params.append('gender', userParams.gender);
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('orderBy', userParams.orderBy.toString());

    return this.getPaginatedResult<Member[]>(this.baseUrl + "users", params).pipe(
      map( response =>{
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;  
      })
    );
  }

  
  getMember(username: string){
    const member = [...this.memberCache.values()]
      .reduce((accu, elem) => accu.concat(elem.result),[])
      .find((member: Member) => member.userName === username);

      if(member) return of(member);
    return this.http.get<Member>(this.baseUrl +'users/'+username);
  }

  updateMember(member){
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() =>{
         const index  = this.members.findIndex[member];
         this.members[index] = member; 
      })
    );
  }

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/'+photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl +'users/delete-photo/'+photoId);
  }

  private getPaginatedResult<T>(url:string, params: HttpParams) {
    const paginationResult: PgainationResult<T> = new PgainationResult<T>();

    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginationResult.result = response.body;
        if (response.headers.get("Pagination") != null) {
          paginationResult.pagination = JSON.parse(response.headers.get("Pagination"));
        }
        //console.log(this.paginationResult.pagination);
        return paginationResult;
      }));
  }

  private generatePaginationHeader(pageNumber: number, pageSize: number)
  {
    let params: HttpParams = new HttpParams();
      params = params.append("PageNumber", pageNumber.toString());
      params = params.append("PageSize", pageSize.toString());

    return params;
  }}