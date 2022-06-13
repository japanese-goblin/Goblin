import { Component, OnInit } from '@angular/core';
import {ItemsResponse} from "../../../models/items-response";
import {BotUserDto} from "../../../models/bot-user-dto";
import {AdminService} from "../../../services/admin.service";
import {UserType} from "../../../models/user-type";

@Component({
  selector: 'app-tg',
  templateUrl: './tg.component.html',
  styleUrls: ['./tg.component.scss']
})
export class TgComponent implements OnInit {

  public data: ItemsResponse<BotUserDto> | undefined;

  constructor(private adminService: AdminService) {
    adminService.getUsers(UserType.telegram).subscribe({
      next: response => this.data = response
    });
  }

  ngOnInit(): void {
  }

}
