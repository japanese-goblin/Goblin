using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using ICalParser.Models;
using Newtonsoft.Json;
using Serilog;

namespace Goblin.Narfu.Schedule
{
    public class StudentsSchedule
    {
        private Group[] Groups { get; }
        private readonly ILogger _logger;

        public StudentsSchedule()
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(StudentsSchedule)).Location);
            Groups = JsonConvert.DeserializeObject<Group[]>(File.ReadAllText($"{path}/Data/Groups.json"));

            _logger = Log.ForContext<StudentsSchedule>();
        }

        public async Task<IEnumerable<Lesson>> GetSchedule(int realGroupId)
        {
            _logger.Debug("Получение расписания для группы {0}", realGroupId);
            var siteGroupId = GetGroupByRealId(realGroupId).SiteId;
            var response = await RequestBuilder.Create()
                                               .SetQueryParam("icalendar")
                                               .SetQueryParam("oid", siteGroupId)
                                               .SetQueryParam("cod", realGroupId)
                                               .SetQueryParam("from", DateTime.Today.ToString("dd.MM.yyyy"))
                                               .GetStringAsync();
            _logger.Debug("Расписание получено");

            var calendar = new vCalendar(response);

            var events = calendar.Events
                                 .Distinct()
                                 .OrderBy(x => x.DtStart);

            return events.Select(ev =>
            {
                var description = ev.Description.Split('\n');
                var address = ev.Location.Split('/');
                return new Lesson
                {
                    Id = ev.Uid,
                    Address = address[0],
                    Auditory = address[1],
                    Number = int.Parse(description[0][0].ToString()),
                    Groups = description[1].Substring(3),
                    Name = ev.Summary,
                    Type = description[3],
                    Teacher = description[4],
                    StartTime = ev.DtStart,
                    EndTime = ev.DtEnd,
                    StartEndTime = description[0].Replace(")", "")
                                                 .Replace("(", "")
                                                 .Replace("п", ")")
                };
            });
        }

        public async Task<ExamsViewModel> GetExams(int realGroupId)
        {
            _logger.Debug("Получение списка экзаменов для группы {0}", realGroupId);
            var schedule = await GetSchedule(realGroupId);
            var exams = schedule.Where(x => x.IsExam());
            _logger.Debug("Список экзаменов получен");

            return new ExamsViewModel(exams, DateTime.Today);
        }

        public async Task<LessonsViewModel> GetScheduleAtDate(int realGroupId, DateTime date)
        {
            _logger.Debug("Получение расписания для группы {0} на {1:dd.MM.yyyy}", realGroupId, date);
            var lessons = await GetSchedule(realGroupId);
            return new LessonsViewModel(lessons.Where(x => x.StartTime.Date == date.Date), date);
        }

        public bool IsCorrectGroup(int realGroupId)
        {
            return Groups.Any(x => x.RealId == realGroupId);
        }

        public Group GetGroupByRealId(int realGroupId)
        {
            return Groups.FirstOrDefault(x => x.RealId == realGroupId);
        }

        public string GenerateScheduleLink(int realGroupId, bool isWebCal = false)
        {
            const string url = "ruz.narfu.ru/?icalendar";

            var protocol = isWebCal ? "webcal" : "https";

            var siteGroupId = GetGroupByRealId(realGroupId).SiteId;

            var todayDate = DateTime.Today.ToString("dd.MM.yyyy");

            return $"{protocol}://{url}&oid={siteGroupId}&cod={realGroupId}&from={todayDate}";
        }
    }
}