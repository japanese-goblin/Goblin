import {Component, Input, OnInit} from '@angular/core';
import {BotUserDto} from "../../../models/bot-user-dto";
import {ItemsResponse} from "../../../models/items-response";

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss']
})
export class TableComponent implements OnInit {

  @Input()
  public data: ItemsResponse<BotUserDto> | undefined;
  constructor() { }

  ngOnInit(): void {
  }

}
