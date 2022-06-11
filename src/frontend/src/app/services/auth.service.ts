import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {Observable} from "rxjs/internal/Observable";
import {catchError, map, of, shareReplay} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    public cache$: Observable<boolean> | undefined; //TOdo:
    
    constructor(private httpClient: HttpClient) {
    }

    public IsAdmin(): Observable<boolean> {
        if (!this.cache$) {
            this.cache$ = this.requestCheck().pipe(
              shareReplay(1)
            );
        }

        return this.cache$;
    }
    
    private requestCheck(): Observable<boolean> {
        return this.httpClient.get<Array<string>>("auth/check").pipe(
          map(response => response.length > 0 && response.includes("admin")),
          catchError(error => of(false))
        );
    }
}
