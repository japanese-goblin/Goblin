import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { ScheduleComponent } from './schedule/schedule.component';
import { StartComponent } from './start/start.component';
import { ScheduleDetailsComponent } from './schedule-details/schedule-details.component';
import { LessonComponent } from './lesson/lesson.component';
import { LessonTypeColorPipe } from './lesson-type-color-pipe';

import { registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
registerLocaleData(localeRu, 'ru');

@NgModule({
    declarations: [
        AppComponent,
        ScheduleComponent,
        StartComponent,
        ScheduleDetailsComponent,
        LessonComponent,
        LessonTypeColorPipe
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
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
