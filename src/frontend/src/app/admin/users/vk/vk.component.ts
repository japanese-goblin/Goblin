import {Component, OnInit} from '@angular/core';
import {AdminService} from "../../../services/admin.service";
import {UserType} from "../../../models/user-type";
import {ItemsResponse} from "../../../models/items-response";
import {BotUserDto} from "../../../models/bot-user-dto";

@Component({
  selector: 'app-vk',
  templateUrl: './vk.component.html',
  styleUrls: ['./vk.component.scss']
})
export class VkComponent implements OnInit {
  public data: ItemsResponse<BotUserDto> | undefined;

  constructor(private adminService: AdminService) {
    adminService.getUsers(UserType.vkontakte).subscribe({
      next: response => this.data = response
    });
  }

  ngOnInit(): void {
  }

}
