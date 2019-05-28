using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using HtmlAgilityPack;
using Narfu.Extensions;
using Narfu.Models;
using Newtonsoft.Json;

namespace Narfu.Schedule
{
    public class TeachersSchedule
    {
        public readonly Teacher[] Teachers;

        public TeachersSchedule()
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TeachersSchedule)).Location); //TODO:
            Teachers = JsonConvert.DeserializeObject<Teacher[]>(File.ReadAllText($"{path}/Data/Teachers.json"));
        }

        public async Task<(bool IsError, Lesson[] Lessons)> GetSchedule(int id)
        {
            HtmlDocument doc;
            try
            {
                //из-за gateway timeout
                var response = await RequestHelper.BuildRequest()
                                                  .SetQueryParam("timetable")
                                                  .SetQueryParam("lecturer", id)
                                                  .GetStreamAsync();
                doc = new HtmlDocument();
                doc.Load(response);
            }
            catch
            {
                return (true, null);
            }

            var lessonItems = doc.DocumentNode.SelectNodes("//div[contains(@class, 'timetable_sheet')]");
            if(lessonItems is null)
            {
                return (true, null);
            }

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
                    Name = lessonNode.SelectSingleNode(".//span[contains(@class,'discipline')]").GetNormalizedInnerText(), 
                    Type = lessonNode.SelectSingleNode(".//span[contains(@class,'kindOfWork')]").GetNormalizedInnerText(), 
                    Teacher = "",
                    StartTime = DateTime.ParseExact($"{date} {time[0]}", "dd.MM.yyyy HH:mm", null, DateTimeStyles.None),
                    EndTime = DateTime.ParseExact($"{date} {time[1]}", "dd.MM.yyyy HH:mm", null, DateTimeStyles.None),
                    StartEndTime = lessonNode.SelectSingleNode(".//span[contains(@class,'time_para')]").GetNormalizedInnerText() 
                });
            }

            return (false, lessons.Distinct().ToArray());
        }

        public async Task<string> GetScheduleAsString(int id)
        {
            var teacher = Teachers.FirstOrDefault(x => x.Id == id);
            if(teacher is null)
            {
                return "Ошибка. Данного преподавателя нет в списке.\n" +
                       "Если Вы уверены, что всё правильно, свяжитесь с администрацией через команду 'админ *сообщение*' " +
                       "для добавления преподавателя в список";
            }

            var (error, lessons) = await GetSchedule(id);

            if(error)
            {
                return "Ошибка.\n" +
                       "Возможно, сайт с расписанием недоступен (либо введен неправильный номер преподавателя)\n" +
                       $"Вы можете проверить расписание здесь: {Constants.EndPoint}/?timetable&lecturer={id}";
            }

            lessons = lessons.Where(x => x.StartTime > DateTime.Today).ToArray();

            if(lessons.Length == 0)
            {
                return "На данный момент у этого преподавателя нет пар";
            }

            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat("Расписание пар у преподавателя '{0}':", teacher.Name).AppendLine();
            foreach(var group in lessons.Where(x => x.StartTime.Date >= DateTime.Now.Date)
                                        .GroupBy(x => x.StartTime.DayOfYear).Take(10))
            {
                strBuilder.AppendFormat("{0:dd.MM (dddd)}", group.First().StartTime).AppendLine();
                foreach(var lesson in group)
                {
                    strBuilder.AppendFormat("{0} - {1} [{2}] ", lesson.StartEndTime, lesson.Name, lesson.Type);
                    strBuilder.AppendFormat("в {0} ({1})", lesson.Auditory, lesson.Address).AppendLine();
                }

                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }

        public string FindByName(string name)
        {
            name = name.ToLower(); //TODO ?
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