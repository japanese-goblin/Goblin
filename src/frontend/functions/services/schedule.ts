import {Group} from "./group";
import {Calendar} from "./ical/calendar";
import {Lesson} from "./lesson";
import {parse} from 'node-html-parser'

export class Schedule {
    Endpoint: string = "https://ruz.narfu.ru/"

    public async getLessons(group: Group, dateString: string): Promise<{ isFromSite: boolean; responseLessons: Record<string, Record<string, Lesson[]>> }> {
        let url = `${this.Endpoint}?icalendar&oid=${group.SiteId}&cod=${group.RealId}&from=${dateString}`;
        let response = await fetch(url);
        let lessons;
        let isFromSite = false;
        if (response.status != 200) {
            lessons = await this.getLessonsFromHtml(group);
            isFromSite = true;
        } else {
            lessons = this.getLessonsFromCalendar(await response.text());
        }
        let uniqueLessons = this.manupulate(lessons);

        let responseLessons = this.formatLessons(uniqueLessons)
        return {isFromSite, responseLessons};
    }
    
    private manupulate(lessons: Lesson[]): Lesson[] {
        let uniqueLessons = [...new Map(lessons.map(l => [l.type + l.name + l.number + l.auditory + l.teacher + l.groups + l.startTime, l])).values()]
        let dates = uniqueLessons.map(x => x.startTime.getTime());
        let firstDay = new Date(Math.min(...dates));
        let lastDay = new Date(Math.max(...dates));
        // @ts-ignore //TODO:
        let diffInDays = (lastDay - firstDay) / (1000 * 60 * 60 * 24);
        for (let i = 0; i <= diffInDays; i++) {
            let day = this.addDays(firstDay, i);
            if(day.getDay() == 0) {
                continue;
            }
            
            let lessonsAtDay = uniqueLessons.filter(x => x.startTime.toDateString() == day.toDateString());
            if(lessonsAtDay.length == 0) {
                uniqueLessons.push({
                    name: 'Выходной',
                    startTime: day,
                    number: 1
                } as Lesson)
                continue;
            }
            
            let maxLessonNumber = Math.max.apply(null, lessonsAtDay.map(x => x.number));
            console.log(maxLessonNumber);
            for (let lessonNumber = 1; lessonNumber <= maxLessonNumber; lessonNumber++) {
                let isPreviousBreak = !lessonsAtDay.filter(x => x.number == lessonNumber -1)[0]?.address && lessonNumber > 1;
                if(lessonsAtDay.filter(x => x.number == lessonNumber).length == 0 && !isPreviousBreak) {
                    uniqueLessons.push({
                        startTime: day,
                        number: lessonNumber,
                        name: lessonNumber == 1 ? "Здоровый сон" : "Перерыв"
                    } as Lesson)
                }
            }
        }
        
        return uniqueLessons.sort((a, b) => a.number - b.number);
    }
    
    public generateLink(group: Group, isWebCal: boolean, date: string): string {
        const base = `ruz.narfu.ru?icalendar`
        let protocol = isWebCal ? "webcal" : "https";
        return `${protocol}://${base}&oid=${group.SiteId}&cod=${group.RealId}&from=${date}`
    }

    private async getLessonsFromHtml(group: Group): Promise<Lesson[]> {
        const regex = /\s{2,}|\\n|\\t/gm;

        let url = `${this.Endpoint}?timetable&group=${group.SiteId}`;
        let response = await fetch(url);
        let text = await response.text();
        let domParser = parse(text);

        let lessonItems = domParser.querySelectorAll('.timetable_sheet.hidden-xs.hidden-sm');
        return lessonItems.filter(x => x.childNodes.length > 3).map(lessonNode => {
            let date = lessonNode.parentNode.querySelector('.dayofweek')?.innerText.replace(regex, "").split(',')[1] ?? '';
            // @ts-ignore //TODO:
            let [auditory, ...address] = lessonNode.querySelector('.auditorium')?.innerText.replace(regex, "").replace("&nbsp;", " ").split(',');
            let finalAddress = address.join(', ');
            let time = lessonNode.querySelector('.time_para')?.innerText.replace(regex, "") ?? '';
            let number = Number(lessonNode.querySelector('.num_para')?.innerText);
            let group = lessonNode.querySelector('.group')?.innerText ?? '';
            let discipline = lessonNode.querySelector('.discipline')?.childNodes[0].textContent.slice(0, -2) ?? '';
            let type = lessonNode.querySelector('.kindOfWork')?.innerText ?? '';
            let teacher = lessonNode.querySelector('.discipline>nobr')?.innerText ?? '';
            let startEnd = lessonNode.querySelector('.time_para')?.innerText.replace(regex, "").replace("&ndash;", " - ") ?? '';
            let link = lessonNode.querySelector('a')?.getAttribute('href');

            let lessonStartTime = startEnd.split(' - ')[0];
            let lessonEndTime = startEnd.split(' - ')[1];

            let startTime = this.convertDateTime(`${date} ${lessonStartTime}`);
            let endTime = this.convertDateTime(`${date} ${lessonEndTime}`);

            let lesson = new Lesson('', type, discipline, startTime, endTime, startEnd, number,
                finalAddress, auditory.slice(5), teacher, group, link);
            
            return lesson;
        });
    }

    private getLessonsFromCalendar(text: string): Lesson[] {
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
                description[4], description[1].slice(3), undefined);
        })
    }

    private convertDateTime(dateTime: string): Date {
        let splittedDate = dateTime.split(' ')[0].split('.');
        let day = Number(splittedDate[0]);
        let month = Number(splittedDate[1]) - 1;
        let year = Number(splittedDate[2]);

        let splittedTime = dateTime.split(' ')[1].split(':');
        let hour = Number(splittedTime[0]);
        let minute = Number(splittedTime[1]);

        return new Date(year, month, day, hour, minute, 0);
    }

    private getStartOfTheDay(date: Date) {
        date.setHours(0, 0, 0, 0);

        return date.toISOString();
    }

    private getStartOfTheWeek(date: Date) {

        const day = date.getDay(); // get day of week
        const firstDayOfTheWeek = 1;
        const dayDifference = (firstDayOfTheWeek - day) * -1;

        const dayDifferenceMs = dayDifference * 24 * 60 * 60 * 1000;

        const newDate = new Date(date.getTime() - dayDifferenceMs);

        return this.getStartOfTheDay(newDate);
    }

    private formatLessons(lessons: Lesson[]): Record<string, Record<string, Lesson[]>> {
        const formattedLessons = {} as any;

        for (let i = 0; i < lessons.length; i++) {
            const lesson = lessons[i];

            const lessonDay = lesson.startTime;
            const lessonDayDate = this.getStartOfTheDay(lessonDay);
            const lessonWeekDate = this.getStartOfTheWeek(lessonDay);

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

    private addDays(date: Date, days: number): Date {
        let result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }
}