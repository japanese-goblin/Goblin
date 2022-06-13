import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpInterceptor,
    HttpErrorResponse
} from '@angular/common/http';
import { ToastService } from './services/toast.service';
import { tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private toastService: ToastService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler) {
        const apiReq = request.clone({
            url: `${environment.apiUrl}/${request.url}`,
            withCredentials: true
        });
        return next.handle(apiReq).pipe(tap({
            error: (e: HttpErrorResponse) => {
                if (e.status == 0) {
                    this.toastService.showError("Сервер временно недоступен. Попробуйте позже.");
                }
                else if (e.status == 400) {
                    this.toastService.showError(e.error.join("\n"));
                }
                else if (e.status == 401) {
                    // skip
                }
                else {
                    this.toastService.showError(e.error.title ?? e.statusText);
                }
            }
        }))
    }
}