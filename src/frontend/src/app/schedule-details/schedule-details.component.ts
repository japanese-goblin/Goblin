import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScheduleResponse } from '../schedule-response';
import { ScheduleServiceService } from '../schedule.service';
import { ToastService } from '../toast.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';


@Component({
    selector: 'app-schedule-details',
    templateUrl: './schedule-details.component.html',
    styleUrls: ['./schedule-details.component.scss'],
})
export class ScheduleDetailsComponent implements OnInit {

    groupId?: Number;
    date?: Date;
    response!: ScheduleResponse;
    pattern: RegExp = /(\d{2})\.(\d{2})\.(\d{4})/;

    constructor(private route: ActivatedRoute,
                private service: ScheduleServiceService,
                private toastService: ToastService,
                private sanitizer: DomSanitizer) {
        let queryDate = this.route.snapshot.queryParamMap.get('date');
        let date = new Date();
        if(queryDate !== null && !queryDate.match(this.pattern)) {
            queryDate.match(this.pattern)
            date = new Date(queryDate.replace(this.pattern, '$3-$2-$1'));
        }
        this.route.params.subscribe(params => {
            this.groupId = params['groupId'];
            this.service.getLessons(this.groupId, date)
                .subscribe({
                    next: response => this.response = response,
                    error: (e: HttpErrorResponse) => {
                        if(e.status == 0) {
                            this.toastService.showError("Сервер с расписанием временно недоступен. Попробуйте позже.");
                        }
                        else if(e.status == 400) {
                            this.toastService.showError(e.error.join("\n"));
                        }
                        else {
                            this.toastService.showError(e.error.title ?? e.statusText);
                        }
                        console.log(e);
                    }
                });
        });
    }

    ngOnInit(): void {
    }

    sanitize(text: string): SafeUrl {
        return this.sanitizer.bypassSecurityTrustUrl(text);
    }
}
