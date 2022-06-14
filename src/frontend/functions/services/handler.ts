import { Group } from './group';
import { Schedule } from './Schedule';
declare const Goblin: KVNamespace;

export async function handleRequest(request: Request): Promise<Response> {
    const params = new Map<string, any>();
    const url = new URL(request.url);
    const queryString = url.search.slice(1).split('&');
  
    queryString.forEach(item => {
      const kv = item.split('=');
      if (kv[0]) { 
          params.set(kv[0], kv[1] || true);
      }
    });

    if(!params.has("group")) {
        return new Response(JSON.stringify(["Нужно указать номер группы (например, 351017)"]), {
            headers: {
                'content-type': 'application/json'
            }
        });
    }

    let queryGroup = params.get('group');
    let groupNumber = parseInt(queryGroup);
    if(isNaN(groupNumber)) {
        return new Response(JSON.stringify(["Номер группы должен быть числом"]), {
            headers: {
                'content-type': 'application/json'
            }
        });
    }

    let kvGroups = await Goblin.get('Groups');
    let narfuGroups = JSON.parse(kvGroups!) as Group[];
    let group = narfuGroups.find((x) => x.RealId == groupNumber);
    if(!group) {
        return new Response(JSON.stringify(narfuGroups), {
            headers: {
                'content-type': 'application/json'
            }
        });
    }

    let schedule = new Schedule();
    let lessons = await schedule.getLessons(group, new Date()); //TODO:

    // return new Response(JSON.stringify(narfuGroups));
    // let s = await fetch("http://ruz.narfu.ru/?icalendar&oid=15594&cod=151111&from=25.04.2022");
    // let api = new Schedule();
    // let lessons = api.getLessons(await s.text());
    return new Response(JSON.stringify(lessons), {
        headers: {
            'content-type': 'application/json'
        }
    });
}