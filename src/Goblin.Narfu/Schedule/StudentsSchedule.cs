using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu.Models;
using Ical.Net;
using Newtonsoft.Json;
using Serilog;

namespace Goblin.Narfu.Schedule
{
    public class StudentsSchedule
    {
        public Group[] Groups { get; }
        private readonly ILogger _logger;
        private readonly IFlurlClient _client;

        public StudentsSchedule(IFlurlClient client)
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(StudentsSchedule)).Location);
            Groups = JsonConvert.DeserializeObject<Group[]>(File.ReadAllText($"{path}/Data/Groups.json"));
            _logger = Log.ForContext<StudentsSchedule>();
            _client = client;
        }

        public async Task<IEnumerable<Lesson>> GetSchedule(int realGroupId)
        {
            _logger.Debug("Получение расписания для группы {0}", realGroupId);
            var siteGroupId = GetGroupByRealId(realGroupId).SiteId;
            var response = await _client.Request()
                                        .SetQueryParam("icalendar")
                                        .SetQueryParam("oid", siteGroupId)
                                        .SetQueryParam("cod", realGroupId)
                                        .SetQueryParam("from", DateTime.Today.ToString("dd.MM.yyyy"))
                                        .GetStreamAsync();
            _logger.Debug("Расписание получено");

            var calendar = Calendar.Load(response);
            var events = calendar.Events
                                 .Distinct()
                                 .OrderBy(x => x.DtStart.Value)
                                 .ToArray();

            if(!events.Any())
            {
                _logger.Debug("Список пар пуст");
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
                    Number = int.Parse(description[0][0].ToString()),
                    Groups = description[1].Substring(3),
                    Name = ev.Summary,
                    Type = description[3],
                    Teacher = description[4],
                    StartTime = ev.DtStart.AsSystemLocal,
                    EndTime = ev.DtEnd.AsSystemLocal,
                    StartEndTime = description[0].Replace(")", "").Replace("(", "").Replace("п", ")")
                };
            });

            return lessons;
        }

        public async Task<ExamsViewModel> GetExams(int realGroupId)
        {
            _logger.Debug("Получение списка экзаменов для группы {0}", realGroupId);
            var schedule = await GetSchedule(realGroupId);
            var exams = schedule.Where(x => x.Type.ToLower().Contains("экзамен") ||
                                            x.Type.ToLower().Contains("зачет"));
            _logger.Debug("Список экзаменов получен");

            return new ExamsViewModel(exams, DateTime.Today);
        }

        public async Task<LessonsViewModel> GetScheduleAtDate(int realGroupId, DateTime date)
        {
            _logger.Debug("Получение расписания для группы {0} на {1:dd.MM.yyyy}", realGroupId, date);
            var schedule = await GetSchedule(realGroupId);
            var lessons = schedule.Where(x => x.StartTime.DayOfYear == date.DayOfYear);
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