import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth.guard';
import { ScheduleDetailsComponent } from './schedule-details/schedule-details.component';
import { ScheduleComponent } from './schedule/schedule.component';
import { StartComponent } from './start/start.component';
import {HomeComponent} from "./admin/home/home.component";
import {VkComponent} from "./admin/users/vk/vk.component";
import {TgComponent} from "./admin/users/tg/tg.component";
import {MessagesComponent} from "./admin/messages/messages.component";

const appRoutes: Routes = [
    {
        path: '',
        component: StartComponent
    },
    {
        path: 'schedule',
        component: ScheduleComponent,
        children: [
            {
                path: ':groupId',
                component: ScheduleDetailsComponent
            }
        ]
    },
    {
        path: 'admin',
        component: HomeComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: 'users/vk',
                component: VkComponent
            },
            {
                path: 'users/telegram',
                component: TgComponent
            },
            {
                path: 'messages',
                component: MessagesComponent
            }
        ]
    }
    // { path: '**', component: PageNotFoundComponent } //TODO:
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }