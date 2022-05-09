import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    title = 'japanese-goblin';
    collapsed = true;
    
    toggleCollapsed(): void {
        this.collapsed = !this.collapsed;
    }

    authUrl: string;
    constructor() {
        let checkAuthRoute = `${window.origin}/auth`
        this.authUrl = `${environment.apiUrl}/auth?returnUrl=${checkAuthRoute}`
    }
}
