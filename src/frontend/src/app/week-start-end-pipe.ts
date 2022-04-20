import { Pipe, PipeTransform } from '@angular/core';
@Pipe({ name: 'weekStartEnd' })
export class WeekStartEndPipe implements PipeTransform {
    transform(value: Date): string {
        let firstDay = new Date(value);
        let lastDay = new Date();
        lastDay.setDate(firstDay.getDate() + 6);

        return `${firstDay.toLocaleDateString('ru-RU')} - ${lastDay.toLocaleDateString('ru-RU')}`
    }
}