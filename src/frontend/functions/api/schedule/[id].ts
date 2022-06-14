import {Schedule} from "../../services/schedule";

export async function onRequest(context: any) {
    // Contents of context object
    const {
        request, // same as existing Worker API
        env, // same as existing Worker API
        params, // if filename includes [id] or [[path]]
        waitUntil, // same as ctx.waitUntil in existing Worker API
        next, // used for middleware or to fetch assets
        data, // arbitrary space for passing data between middlewares
    } = context;

    let schedule = new Schedule();
    let lessons = await schedule.getLessons({RealId: 351017, SiteId: 15085, Name: ''}); //TODO:
    let response = {
        groupName: "test",
        groupId: 12356,
        lessons: lessons,
        webCalLink: "webcal://ruz.narfu.ru/?icalendar&oid=15086&cod=351018&from=14.06.2022",
        icsLink: "https://ruz.narfu.ru/?icalendar&oid=15086&cod=351018&from=14.06.2022",
        isFromSite: true
    }

    return new Response(JSON.stringify(response), {
        headers: {
            'content-type': 'application/json'
        }
    });
}
