import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatToolbarModule } from '@angular/material/toolbar'
import { MatIconModule } from '@angular/material/icon'
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle'; 
import { MatInputModule } from '@angular/material/input'; 
import { MatDatepickerModule } from '@angular/material/datepicker'; 
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';

import { ScheduleComponent } from './schedule/schedule.component'
import { StartComponent } from './start/start.component';
import { ScheduleDetailsComponent } from './schedule-details/schedule-details.component';

import { MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    ScheduleComponent,
    StartComponent,
    ScheduleDetailsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatInputModule,
    MatDatepickerModule,
    FormsModule, 
    ReactiveFormsModule,
    MatMenuModule,
    MatDividerModule
  ],
  providers: [
      { provide: MAT_DATE_LOCALE, useValue: 'ru-RU' },
      { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
      { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }