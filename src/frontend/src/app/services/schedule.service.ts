import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { ScheduleResponse } from '../models/schedule-response';
import {environment} from "../../environments/environment";

@Injectable({
    providedIn: 'root'
})
export class ScheduleServiceService {
    constructor(private httpClient: HttpClient) {
    }

    getLessons(groupId: Number | undefined, date: Date | undefined): Observable<ScheduleResponse> {
        let requestUrl = `${environment.apiUrl}/api/schedule/${groupId}?date=${date?.toLocaleDateString('ru-RU')}`;
        return this.httpClient.get<ScheduleResponse>(requestUrl);
    }
}
