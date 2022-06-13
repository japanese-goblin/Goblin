import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {NgbModule, NgbNavModule} from '@ng-bootstrap/ng-bootstrap';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {CommonModule, registerLocaleData} from '@angular/common';
import {ScheduleComponent} from './schedule/schedule.component';
import {StartComponent} from './start/start.component';
import {ScheduleDetailsComponent} from './schedule-details/schedule-details.component';
import {LessonComponent} from './lesson/lesson.component';
import {LessonTypeColorPipe} from './pipes/lesson-type-color-pipe';
import {WeekStartEndPipe} from './pipes/week-start-end-pipe';
import localeRu from '@angular/common/locales/ru';
import {ToastContainerComponent} from './toast-container/toast-container.component';
import {AuthInterceptor} from './auth.interceptor';
import {HomeComponent} from './admin/home/home.component';
import {VkComponent} from './admin/users/vk/vk.component';
import {TableComponent} from "./admin/users/table/table.component";
import {TgComponent} from './admin/users/tg/tg.component';
import {MessagesComponent} from './admin/messages/messages.component';
import {NgxGoogleAnalyticsModule, NgxGoogleAnalyticsRouterModule} from "ngx-google-analytics";

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
        HomeComponent,
        VkComponent,
        TableComponent,
        TgComponent,
        MessagesComponent,
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
        NgbNavModule,
        NgxGoogleAnalyticsModule.forRoot('G-XP0G1G0E7J'),
        NgxGoogleAnalyticsRouterModule.forRoot({exclude: ['/admin/*']})
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
