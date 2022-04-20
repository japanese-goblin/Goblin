import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { throwError } from 'rxjs';
import { ScheduleResponse } from '../schedule-response';
import { ScheduleServiceService } from '../schedule.service';
import { ToastService } from '../toast.service';

@Component({
    selector: 'app-schedule-details',
    templateUrl: './schedule-details.component.html',
    styleUrls: ['./schedule-details.component.scss'],
})
export class ScheduleDetailsComponent implements OnInit {

    groupId?: Number;
    date?: Date;
    response!: ScheduleResponse;

    constructor(private route: ActivatedRoute,
                private service: ScheduleServiceService,
                private toastService: ToastService) {
        let queryDate = this.route.snapshot.queryParamMap.get('date');
        let date = new Date();
        if(queryDate !== null) {
            var pattern = /(\d{2})\.(\d{2})\.(\d{4})/;
            date = new Date(queryDate.replace(pattern,'$3-$2-$1'));
        }
        this.route.params.subscribe(params => {
            this.groupId = params['groupId'];
            this.service.getLessons(this.groupId, date)
                .subscribe({
                    next: response => this.response = response,
                    error: (e) => this.toastService.showError(e.error.title ?? e.statusText)
                });
        });
    }

    ngOnInit(): void {
    }
}
