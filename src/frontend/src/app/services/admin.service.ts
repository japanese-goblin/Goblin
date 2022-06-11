import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {UserType} from "../models/user-type";
import {Observable} from "rxjs/internal/Observable";
import {ItemsResponse} from "../models/items-response";
import {BotUserDto} from "../models/bot-user-dto";

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private httpClient: HttpClient) {
  }
  
  getUsers(type: UserType): Observable<ItemsResponse<BotUserDto>> {
    let requestUrl = `api/admin/users?Consumer=${type}`;
    return this.httpClient.get<ItemsResponse<BotUserDto>>(requestUrl);
  }
}
