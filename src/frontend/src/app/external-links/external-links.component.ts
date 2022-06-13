import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-external-links',
  template: `
    <div class="gap-2 btn-group-lg mx-auto" role="group">
      <a class="btn" href="https://vk.com/japanese.goblin" target="_blank">
        <img class="mx-auto" src="assets/vk.svg" alt="" width="60" height="60">
      </a>
      <a class="btn" href="https://t.me/japanese_goblin_bot" target="_blank">
        <img class="mx-auto" src="assets/telegram.svg" alt="" width="60" height="60">
      </a>
    </div>
    
  `,
  styles: [
  ]
})
export class ExternalLinksComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
