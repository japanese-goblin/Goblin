import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScheduleResponse } from '../schedule-response';
import { ScheduleServiceService } from '../schedule-service.service';
import * as moment from 'moment';

@Component({
    selector: 'app-schedule-details',
    templateUrl: './schedule-details.component.html',
    styleUrls: ['./schedule-details.component.scss']
})
export class ScheduleDetailsComponent implements OnInit {

    groupId?: Number;
    date?: Date;
    response!: ScheduleResponse;

    constructor(private route: ActivatedRoute, private service: ScheduleServiceService) {
        this.route.params.subscribe(params => {
            this.groupId = params['groupId'];
            this.service.getLessons(this.groupId, undefined).subscribe(response => {
                this.response = response;
            });
            // this.date = params['date'];
        });
    }

    ngOnInit(): void {
    }
}
