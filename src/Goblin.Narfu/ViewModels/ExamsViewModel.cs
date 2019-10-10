using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels
{
    public class ExamsViewModel : LessonsViewModel
    {
        public ExamsViewModel(IEnumerable<Lesson> lessons, DateTime date) : base(lessons, date)
        {
        }

        public override string ToString()
        {
            var exams = Lessons.Where(x => x.StartTime.DayOfYear >= _date.DayOfYear).ToArray();
            if(!exams.Any())
            {
                return "На данный момент список экзаменов отсутствует";
            }

            var strBuilder = new StringBuilder();
            var grouped = exams.GroupBy(x => x.StartTime.Date);

            foreach(var group in grouped)
            {
                strBuilder.AppendFormat("{0:D}:", group.Key).AppendLine();
                foreach(var lesson in group)
                {
                    strBuilder.AppendFormat("{0} - {1} [{2}] ({3})",
                                            lesson.StartEndTime, lesson.Name, lesson.Type, lesson.Teacher)
                              .AppendLine()
                              .AppendFormat("У группы {0}", lesson.Groups).AppendLine()
                              .AppendFormat("В аудитории {0} ({1})", lesson.Auditory, lesson.Address).AppendLine();
                }

                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }
    }
}