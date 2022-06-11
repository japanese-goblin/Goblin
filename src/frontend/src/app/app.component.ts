import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';
import {AuthService} from "./services/auth.service";
import {Observable} from "rxjs/internal/Observable";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    title = 'japanese-goblin';
    collapsed = true;
    authUrl: string;
    isAdmin: Observable<boolean>;

    constructor(public authService: AuthService) {
        let checkAuthRoute = `${window.origin}/`
        this.isAdmin = this.authService.IsAdmin();
        this.authUrl = `${environment.apiUrl}/auth?returnUrl=${checkAuthRoute}`
    }

    toggleCollapsed(): void {
        this.collapsed = !this.collapsed;
    }
}
