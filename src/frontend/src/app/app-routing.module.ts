import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleDetailsComponent } from './schedule-details/schedule-details.component';
import { ScheduleComponent } from './schedule/schedule.component';
import { StartComponent } from './start/start.component';

const appRoutes: Routes = [
    {
        path: 'schedule',
        component: ScheduleComponent
    },
    {
        path: '',
        component: StartComponent
    },
    {
        path: 'schedule/:groupId',
        component: ScheduleDetailsComponent
    }
    // { path: '**', component: PageNotFoundComponent } //TODO:
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }