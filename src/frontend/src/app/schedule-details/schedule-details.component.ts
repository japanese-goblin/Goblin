import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-schedule-details',
    templateUrl: './schedule-details.component.html',
    styleUrls: ['./schedule-details.component.scss']
})
export class ScheduleDetailsComponent implements OnInit {

    groupId?: Number;
    date?: Date;
    constructor(private route: ActivatedRoute) {
        this.route.params.subscribe(params => {
            this.groupId = params['groupId'];
            this.date = params['date'];
        });
    }

    ngOnInit(): void {
    }
}
