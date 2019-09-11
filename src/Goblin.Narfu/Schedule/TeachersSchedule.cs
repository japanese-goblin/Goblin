using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu.Extensions;
using Goblin.Narfu.Models;
using HtmlAgilityPack;
using Serilog;

namespace Goblin.Narfu.Schedule
{
    public class TeachersSchedule
    {
        private readonly ILogger _logger;
        private readonly IFlurlClient _client;

        public TeachersSchedule(IFlurlClient client)
        {
            _logger = Log.ForContext<StudentsSchedule>();
            _client = client;
        }

        public async Task<IEnumerable<Lesson>> GetSchedule(int teacherId)
        {
            _logger.Debug("Получение списка пар у преподавателя {0}", teacherId);
            var response = await _client.Request()
                                        .SetQueryParam("timetable")
                                        .SetQueryParam("lecturer", teacherId)
                                        .GetStreamAsync();
            var doc = new HtmlDocument();
            doc.Load(response);
            _logger.Debug("Список получен");

            var lessonItems = doc.DocumentNode.SelectNodes("//div[contains(@class, 'timetable_sheet')]");

            var lessons = new List<Lesson>();

            foreach(var lessonNode in lessonItems.Where(x => x.ChildNodes.Count > 3))
            {
                var date = lessonNode.ParentNode.SelectSingleNode(".//div[contains(@class,'dayofweek')]")
                                     .GetNormalizedInnerText()
                                     .Split(',', 2)[1]
                                     .Trim();

                var adr = lessonNode.SelectSingleNode(".//span[contains(@class,'auditorium')]")
                                    .GetNormalizedInnerText()
                                    .Split(',', 2)
                                    .Select(x => x.Trim())
                                    .ToArray();

                var time = lessonNode.SelectSingleNode(".//span[contains(@class,'time_para')]")
                                     .GetNormalizedInnerText()
                                     .Split('–', 2);

                lessons.Add(new Lesson
                {
                    Address = adr[1],
                    Auditory = adr[0],
                    Number = int.Parse(lessonNode.SelectSingleNode(".//span[contains(@class,'num_para')]")
                                                 .GetNormalizedInnerText()),
                    Groups = lessonNode.SelectSingleNode(".//span[contains(@class,'group')]").GetNormalizedInnerText(),
                    Name = lessonNode.SelectSingleNode(".//span[contains(@class,'discipline')]")
                                     .GetNormalizedInnerText(),
                    Type = lessonNode.SelectSingleNode(".//span[contains(@class,'kindOfWork')]")
                                     .GetNormalizedInnerText(),
                    Teacher = "",
                    StartTime = DateTime.ParseExact($"{date} {time[0]}", "dd.MM.yyyy HH:mm", null, DateTimeStyles.None),
                    EndTime = DateTime.ParseExact($"{date} {time[1]}", "dd.MM.yyyy HH:mm", null, DateTimeStyles.None),
                    StartEndTime = lessonNode.SelectSingleNode(".//span[contains(@class,'time_para')]")
                                             .GetNormalizedInnerText()
                });
            }
            
            return lessons.Distinct();
        }

        public async Task<TeacherLessonsViewModel> GetLimitedSchedule(int teacherId, int limit = 10)
        {
            var lessons = await GetSchedule(teacherId);
            return new TeacherLessonsViewModel(lessons.Take(10), DateTime.Now);
        }

        public async Task<Teacher[]> FindByName(string name)
        {
            _logger.Debug("Поиск преподавателя {0}", name);
            var teachers = await _client.Request()
                                        .AppendPathSegments("i", "ac.php")
                                        .SetQueryParam("term", name)
                                        .GetJsonAsync<Teacher[]>();
            _logger.Debug("Поиск завершен");

            return teachers;
        }

        public async Task<bool> FindById(int teacherId)
        {
            _logger.Debug("Поиск преподавателя по ID {0}", teacherId);
            const string NotFound = "Данные о расписании на эту неделю отсутствуют в системе.";
            var response = await _client.Request()
                                        .SetQueryParam("timetable")
                                        .SetQueryParam("lecturer", teacherId)
                                        .AllowAnyHttpStatus()
                                        .GetStringAsync();
            _logger.Debug("Поиск завершен");

            return !response.Contains(NotFound);
        }
    }
}