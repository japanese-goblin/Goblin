using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu.Models;
using Ical.Net;
using Newtonsoft.Json;

namespace Goblin.Narfu.Schedule
{
    public class StudentsSchedule
    {
        public Group[] Groups { get; }

        public StudentsSchedule()
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(StudentsSchedule)).Location);
            Groups = JsonConvert.DeserializeObject<Group[]>(File.ReadAllText($"{path}/Data/Groups.json"));
        }

        public async Task<Lesson[]> GetSchedule(int realGroupId)
        {
            var siteGroupId = GetGroupByRealId(realGroupId);
            var response = await Defaults.BuildRequest()
                                         .SetQueryParam("icalendar")
                                         .SetQueryParam("oid", siteGroupId)
                                         .SetQueryParam("cod", realGroupId)
                                         .SetQueryParam("from", DateTime.Today.ToString("dd.MM.yyyy"))
                                         .GetAsync();

            var calendar = Calendar.Load(await response.Content.ReadAsStreamAsync());
            var events = calendar.Events
                                 .Distinct()
                                 .OrderBy(x => x.DtStart.Value)
                                 .ToArray();

            if(!events.Any())
            {
                return new Lesson[] { };
            }

            var lessons = events.Select(ev =>
            {
                var description = ev.Description.Split('\n');
                var address = ev.Location.Split('/');
                return new Lesson
                {
                    Address = address[0],
                    Auditory = address[1],
                    Number = (byte) description[0].ElementAt(0),
                    Groups = description[1].Substring(3),
                    Name = description[2],
                    Type = description[3],
                    Teacher = description[4],
                    StartTime = ev.DtStart.AsSystemLocal,
                    EndTime = ev.DtEnd.AsSystemLocal,
                    StartEndTime = description[0].Replace(")", "").Replace("(", "").Replace("п", ")")
                };
            }).ToArray();

            return lessons;
        }

        public async Task<LessonsViewModel> GetExams(int realGroupId)
        {
            var exams = (await GetSchedule(realGroupId)).Where(x => x.Type.ToLower().Contains("экзамен") ||
                                                                    x.Type.ToLower().Contains("зачет"));
            return new LessonsViewModel(exams, DateTime.Today);
        }

        public async Task<LessonsViewModel> GetScheduleAtDate(int realGroupId, DateTime date)
        {
            var lessons = (await GetSchedule(realGroupId)).Where(x => x.StartTime.DayOfYear == date.DayOfYear);
            return new LessonsViewModel(lessons, date);
        }

        public bool IsCorrectGroup(int realGroupId)
        {
            return Groups.Any(x => x.RealId == realGroupId);
        }

        public Group GetGroupByRealId(int realGroupId)
        {
            return Groups.FirstOrDefault(x => x.RealId == realGroupId);
        }

        public Group GetGroupBySiteId(short siteGroupId)
        {
            return Groups.FirstOrDefault(x => x.SiteId == siteGroupId);
        }
    }
}