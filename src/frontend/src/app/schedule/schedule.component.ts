import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';


@Component({
    selector: 'app-schedule',
    templateUrl: './schedule.component.html',
    styleUrls: ['./schedule.component.scss'],
    providers: [],
})
export class ScheduleComponent implements OnInit {

    minDate: Date;
    checkoutForm = this.formBuilder.group({
        groupId: 0,
        date: moment()
    });

    constructor(private formBuilder: FormBuilder, private router: Router) {
        const thisYear = new Date().getFullYear();
        this.minDate = new Date(thisYear - 1, 8, 1);
    }

    ngOnInit(): void {
    }

    onSubmit(): void {
        if(!this.checkoutForm.valid) {
            return;
        }

        let groupId = this.checkoutForm.value.groupId;
        let date = this.checkoutForm.value.date.format("DD.MM.YYYY");
        this.router.navigate(['/schedule', groupId], { queryParams: { date } });
    }
}
