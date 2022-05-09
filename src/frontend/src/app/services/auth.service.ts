import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private authorized: boolean;
    private isAdmin: boolean;

    constructor(private httpClient: HttpClient) {
        this.authorized = false;
        this.isAdmin = false;
        this.Check();
    }

    private Check(): void {
        this.httpClient.get<Array<string>>("auth/check").subscribe({
            next: response => {
                this.authorized = response.length > 0;
                this.isAdmin = response.includes("admin")
            },
            error: () => {
                this.authorized = false;
                this.isAdmin = false;
            }
        });
    }

    public get Authorized(): boolean {
        return this.authorized;
    }

    public set Authorized(val: boolean) {
        this.authorized = val;
    }

    public get IsAdmin(): boolean {
        return this.authorized;
    }
}
