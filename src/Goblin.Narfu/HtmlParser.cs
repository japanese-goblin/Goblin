using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Goblin.Narfu.Extensions;
using Goblin.Narfu.Models;
using HtmlAgilityPack;

namespace Goblin.Narfu
{
    public static class HtmlParser
    {
        public static IEnumerable<Lesson> GetAllLessonsFromHtml(Stream stream)
        {
            var doc = new HtmlDocument();
            doc.Load(stream);

            var lessonItems = doc.DocumentNode.SelectNodes("//div[contains(@class, 'timetable_sheet hidden-xs hidden-sm')]");

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

                var number = int.Parse(lessonNode.SelectSingleNode(".//span[contains(@class,'num_para')]")
                                                 .GetNormalizedInnerText());

                var groups = lessonNode.SelectSingleNode(".//span[contains(@class,'group')]")
                                       ?.GetNormalizedInnerText();

                var name = lessonNode.SelectSingleNode(".//span[contains(@class,'discipline')]")
                                     .GetNormalizedInnerText();

                var type = lessonNode.SelectSingleNode(".//span[contains(@class,'kindOfWork')]")
                                     .GetNormalizedInnerText();

                var teacher = lessonNode.SelectSingleNode(".//span[contains(@class,'discipline')]//nobr")
                                        ?.GetNormalizedInnerText();

                var startEnd = lessonNode.SelectSingleNode(".//span[contains(@class,'time_para')]")
                                         .GetNormalizedInnerText();

                var lesson = new Lesson
                {
                    Address = adr[1],
                    Auditory = adr[0],
                    Number = number,
                    Groups = groups,
                    Name = name,
                    Type = type,
                    Teacher = teacher,
                    StartTime = DateTime.ParseExact($"{date} {time[0]}", "dd.MM.yyyy HH:mm", null, DateTimeStyles.None),
                    EndTime = DateTime.ParseExact($"{date} {time[1]}", "dd.MM.yyyy HH:mm", null, DateTimeStyles.None),
                    StartEndTime = startEnd
                };

                yield return lesson;
            }
        }
    }
}