using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu.Extensions;
using Goblin.Narfu.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Goblin.Narfu.Schedule
{
    public class TeachersSchedule
    {
        public Teacher[] Teachers { get; }

        public TeachersSchedule()
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TeachersSchedule)).Location);
            Teachers = JsonConvert.DeserializeObject<Teacher[]>(File.ReadAllText($"{path}/Data/Teachers.json"));
        }

        public async Task<Lesson[]> GetSchedule(int teacherId)
        {
            var response = await Defaults.BuildRequest()
                                         .SetQueryParam("timetable")
                                         .SetQueryParam("lecturer", teacherId)
                                         .GetStreamAsync();
            var doc = new HtmlDocument();
            doc.Load(response);
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
                    Number = byte.Parse(lessonNode.SelectSingleNode(".//span[contains(@class,'num_para')]")
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

            return lessons.ToArray();
        }

        public async Task<LessonsViewModel> GetLimitedSchedule(int teacherId, int limit = 10)
        {
            var lessons = await GetSchedule(teacherId);
            return new LessonsViewModel(lessons.Take(10), DateTime.Now);
        }

        public string FindByName(string name)
        {
            name = name.ToLower();
            var teachers = Teachers.Where(x => x.Name.ToLower().Contains(name));
            return string.Join("\n", teachers
                                       .Select(x => $"{x.Name} ({x.Depart}) - {x.Id}"));
        }

        public bool FindById(int id)
        {
            return Teachers.Any(x => x.Id == id);
        }
    }
}