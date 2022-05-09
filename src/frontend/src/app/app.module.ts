import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { ScheduleComponent } from './schedule/schedule.component';
import { StartComponent } from './start/start.component';
import { ScheduleDetailsComponent } from './schedule-details/schedule-details.component';
import { LessonComponent } from './lesson/lesson.component';
import { LessonTypeColorPipe } from './pipes/lesson-type-color-pipe';
import { WeekStartEndPipe } from './pipes/week-start-end-pipe';

import { registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
import { ToastContainerComponent } from './toast-container/toast-container.component';
import { AuthInterceptor } from './auth.interceptor';
import { AuthComponent } from './auth/auth.component';
registerLocaleData(localeRu, 'ru');

@NgModule({
    declarations: [
        AppComponent,
        ScheduleComponent,
        StartComponent,
        ScheduleDetailsComponent,
        LessonComponent,
        LessonTypeColorPipe,
        WeekStartEndPipe,
        ToastContainerComponent,
        AuthComponent
    ],
    imports: [
        CommonModule,
        BrowserModule,
        AppRoutingModule,
        NgbModule,
        NgbNavModule,

        FormsModule,
        ReactiveFormsModule,

        HttpClientModule,

        NgbNavModule
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
