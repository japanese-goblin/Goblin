import {Group} from "./group";
import {Calendar} from "./ical/calendar";
import {Lesson} from "./lesson";
import {LessonType} from "./lesson_type";
import {parse} from 'node-html-parser'

export class Schedule {
    Endpoint: string = "https://ruz.narfu.ru/"

    async getLessons(group: Group, date: Date = new Date()): Promise<Record<string, Record<string, Lesson[]>>> {
        let dateString = date.toLocaleDateString('ru-RU');
        let url = `${this.Endpoint}?icalendar&oid=${group.SiteId}&cod=${group.RealId}&from=${dateString}`;
        let response = await fetch(url);
        let lessons;
        if (response.status != 200) {
            lessons = await this.getLessonsFromHtml(group);
        } else {
            lessons = this.getLessonsFromCalendar(await response.text());
        }
        let uniqueLessons = [...new Map(lessons.map(l => [l.type + l.name + l.number + l.auditory + l.teacher + l.groups, l])).values()]

        return this.formatLessons(uniqueLessons);
    }

    async getLessonsFromHtml(group: Group): Promise<Lesson[]> {
        const regex = /\s{2,}|\\n|\\t/gm;

        let url = `${this.Endpoint}?timetable&group=${group.SiteId}`;
        let response = await fetch(url);
        let text = await response.text();
        let domParser = parse(text);

        let lessonItems = domParser.querySelectorAll('.timetable_sheet.hidden-xs.hidden-sm');
        return lessonItems.filter(x => x.childNodes.length > 3).map(lessonNode => {
            let date = lessonNode.parentNode.querySelector('.dayofweek')?.innerText.replace(regex, "").split(',')[1] ?? '';
            let adr = lessonNode.querySelector('.auditorium')?.innerText.replace(regex, "").replace("&nbsp;", " ").split(',') ?? ['', ''];
            let time = lessonNode.querySelector('.time_para')?.innerText.replace(regex, "") ?? '';
            let number = Number(lessonNode.querySelector('.num_para')?.innerText);
            let group = lessonNode.querySelector('.group')?.innerText ?? '';
            let discipline = lessonNode.querySelector('.discipline')?.innerText ?? '';
            let type = lessonNode.querySelector('.kindOfWork')?.innerText ?? '';
            let teacher = lessonNode.querySelector('.discipline>nobr')?.innerText ?? '';
            let startEnd = lessonNode.querySelector('.time_para')?.innerText.replace(regex, "").replace("&ndash;", " - ") ?? '';
            let link = lessonNode.querySelector('a')?.getAttribute('href');

            let lessonStartTime = startEnd.split(' - ')[0];
            let lessonEndTime = startEnd.split(' - ')[1];

            let startTime = this.convertDateTime(`${date} ${lessonStartTime}`);
            let endTime = this.convertDateTime(`${date} ${lessonEndTime}`);

            return new Lesson('', type, discipline, startTime, endTime, startEnd, number, adr[0], adr[1], teacher, group, LessonType.exam, link)
        });
    }

    getLessonsFromCalendar(text: string): Lesson[] {
        let calendar = new Calendar(text);
        let events = calendar.events.sort((a, b) => a.dtStart.getTime() - b.dtStart.getTime());
        return events.map(ev => {
            let description = ev.description.split('\\n');
            let address = ev.location.split('/');

            let number = parseInt(description[0][0]);
            if (isNaN(number)) {
                number = 1;
            }

            return new Lesson(ev.uid, description[3], ev.summary.replaceAll(".", ". "),
                ev.dtStart, ev.dtEnd, "", number, address[0], address[1],
                description[4], description[1].slice(3), LessonType.consultation, undefined);
        })
    }

    convertDateTime(dateTime: string): Date {
        let splittedDate = dateTime.split(' ')[0].split('.');
        let day = Number(splittedDate[0]);
        let month = Number(splittedDate[1]) - 1;
        let year = Number(splittedDate[2]);

        let splittedTime = dateTime.split(' ')[1].split(':');
        let hour = Number(splittedTime[0]);
        let minute = Number(splittedTime[1]);

        return new Date(year, month, day, hour, minute, 0);
    }

    getStartOfTheDay(date: Date) {
        date.setHours(0, 0, 0, 0);

        return date.toLocaleDateString('ru', {
            year: "numeric",
            month: "2-digit",
            day: "numeric"
        });
    }

    getStartOfTheWeek(date: Date) {

        const day = date.getDay(); // get day of week
        const firstDayOfTheWeek = 1;
        const dayDifference = (firstDayOfTheWeek - day) * -1;

        const dayDifferenceMs = dayDifference * 24 * 60 * 60 * 1000;

        const newDate = new Date(date.getTime() - dayDifferenceMs);

        return this.getStartOfTheDay(newDate);
    }

    formatLessons(lessons: Lesson[]): Record<string, Record<string, Lesson[]>> {
        const formattedLessons = {} as any;

        for (let i = 0; i < lessons.length; i++) {
            const lesson = lessons[i];

            const lessonDay = lesson.startTime;
            const lessonDayDate = this.getStartOfTheDay(lessonDay);
            const lessonWeekDate = this.getStartOfTheWeek(lessonDay);

            // console.log(lessonDayDate, lessonWeekDate);

            if (!formattedLessons.hasOwnProperty(lessonWeekDate)) {
                formattedLessons[lessonWeekDate] = {};
            }

            if (!formattedLessons[lessonWeekDate].hasOwnProperty(lessonDayDate)) {
                formattedLessons[lessonWeekDate][lessonDayDate] = [];
            }

            formattedLessons[lessonWeekDate][lessonDayDate].push(lesson);
        }

        return formattedLessons as Record<string, Record<string, Lesson[]>>;
    }
}