import {Schedule} from "../../services/schedule";
import {Group} from "../../services/group";

export async function onRequest(context: any): Promise<Response> {
    // Contents of context object
    const {
        request, // same as existing Worker API
        env, // same as existing Worker API
        params, // if filename includes [id] or [[path]]
        waitUntil, // same as ctx.waitUntil in existing Worker API
        next, // used for middleware or to fetch assets
        data, // arbitrary space for passing data between middlewares
    } = context;

    let groupId = params.id;
    if (!groupId || isNaN(groupId)) {
        return new Response(JSON.stringify(["Нужно указать номер группы (например, 351017)"]), {
            headers: {
                'content-type': 'application/json'
            }
        });
    }

    let date = getDate(request);

    let group = await getGroup(groupId, env);
    if (!group) {
        const KV = env.Goblin as KVNamespace;
        let kvGroups = await KV.get('Groups');
        let narfuGroups = JSON.parse(kvGroups!) as Group[];
        return new Response(JSON.stringify(narfuGroups), {
            headers: {
                'content-type': 'application/json'
            }
        });
    }

    let schedule = new Schedule();
    let lessons = await schedule.getLessons({RealId: groupId, SiteId: 15085, Name: ''}, date); //TODO:
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

function getDate(request: any) {
    const {searchParams} = new URL(request.url);

    return searchParams.get("date") ??
        new Date().toLocaleDateString('ru', {
            year: "numeric",
            month: "2-digit",
            day: "numeric"
        });
}

async function getGroup(groupId: number, env: any) {
    const KV = env.Goblin as KVNamespace;
    let kvGroups = await KV.get('Groups');
    console.log(kvGroups);
    let narfuGroups = JSON.parse(kvGroups!) as Group[] || [] as Group[];
    return narfuGroups.find((x) => x.RealId == groupId);
}