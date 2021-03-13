import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PgainationResult } from "../_models/pagination";

  export function getPaginatedResult<T>(url:string, params: HttpParams, http: HttpClient) {
    const paginationResult: PgainationResult<T> = new PgainationResult<T>();

    return http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginationResult.result = response.body;
        if (response.headers.get("Pagination") != null) {
          paginationResult.pagination = JSON.parse(response.headers.get("Pagination"));
        }
        //console.log(this.paginationResult.pagination);
        return paginationResult;
      }));
  }

  export function generatePaginationHeader(pageNumber: number, pageSize: number)
  {
    let params: HttpParams = new HttpParams();
      params = params.append("PageNumber", pageNumber.toString());
      params = params.append("PageSize", pageSize.toString());

    return params;
  }