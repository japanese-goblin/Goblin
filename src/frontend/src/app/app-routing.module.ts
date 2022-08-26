import {Injectable, NgModule} from '@angular/core';
import {ActivatedRouteSnapshot, RouterModule, Routes} from '@angular/router';
import {ScheduleDetailsComponent} from './schedule-details/schedule-details.component';
import {ScheduleComponent} from './schedule/schedule.component';
import {StartComponent} from './start/start.component';
import {PageNotFoundComponent} from "./page-not-found/page-not-found.component";

@Injectable({ providedIn: 'root' })
export class GroupTitleResolver {
    resolve(route: ActivatedRouteSnapshot) {
        const userId = route.paramMap.get('groupId');
        console.log(route.paramMap)
        return `Расписание ${userId} - Японский Гоблин`;
    }
}

const appRoutes: Routes = [
    {
        path: '',
        component: StartComponent,
        title: "Японский гоблин"
    },
    {
        path: 'schedule',
        component: ScheduleComponent,
        children: [
            {
                path: ':groupId',
                component: ScheduleDetailsComponent,
                title: GroupTitleResolver
            }
        ],
        title: "Расписание - Японский гоблин"
    },
    {
        path: '**',
        component: PageNotFoundComponent,
        title: "404 - Японский гоблин"
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
}