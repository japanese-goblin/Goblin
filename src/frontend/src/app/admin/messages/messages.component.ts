import {Component, OnInit} from '@angular/core';
import {FormBuilder} from "@angular/forms";
import {UserType} from "../../models/user-type";
import {AdminService} from "../../services/admin.service";

@Component({
    selector: 'app-messages',
    templateUrl: './messages.component.html',
    styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
    public allSendForm = this.formBuilder.group({
        consumer: UserType.vkontakte,
        message: '',
        sendDefaultKeyboard: false
    });


    public oneSendForm = this.formBuilder.group({
        consumer: UserType.vkontakte,
        message: '',
        userId: null
    });

    constructor(private formBuilder: FormBuilder,
                private adminService: AdminService) {
    }

    ngOnInit(): void {
    }


    onAllSendSubmit() {
        this.adminService.sendMessage(this.allSendForm.value.consumer,
            this.allSendForm.value.message,
            this.allSendForm.value.sendDefaultKeyboard).subscribe({
            next: response => console.log(response)
        });
    }

    onOneSendSubmit() {
        this.adminService.sendMessage(this.oneSendForm.value.consumer,
            this.oneSendForm.value.message,
            true,
            this.oneSendForm.value.userId).subscribe(
                {
                  next: response => console.log(response)
                });
    }
}
