import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { ScheduleResponse } from './schedule-response';
import { environment } from '../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ScheduleServiceService {

    private url: string;

    constructor(private httpClient: HttpClient) {
        this.url = environment.apiUrl;
     }

    getLessons(groupId: Number | undefined, date: Date | undefined): Observable<ScheduleResponse> {
        let requestUrl = `${this.url}/api/schedule/${groupId}?date=${date?.toLocaleDateString('ru-RU')}`;
        // let requestUrl = `assets/response.json`;
        return this.httpClient.get<ScheduleResponse>(requestUrl);
    }
}
