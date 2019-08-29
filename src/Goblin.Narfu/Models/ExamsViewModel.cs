using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goblin.Narfu.Models
{
    public class ExamsViewModel : LessonsViewModel
    {
        public ExamsViewModel(IEnumerable<Lesson> lessons, DateTime date) : base(lessons, date)
        {
        }

        public override string ToString()
        {
            if(!Lessons.Any())
            {
                return "На данный момент список экзаменов отсутствует";
            }

            var strBuilder = new StringBuilder();

            foreach(var lesson in Lessons.Where(x => x.StartTime.DayOfYear >= _date.DayOfYear))
            {
                strBuilder.AppendFormat("{0} - {1} [{2}] ({3})",
                                        lesson.StartEndTime, lesson.Name, lesson.Type, lesson.Teacher)
                          .AppendLine()
                          .AppendFormat("У группы {0}", lesson.Groups).AppendLine()
                          .AppendFormat("В аудитории {0} ({1})", lesson.Auditory, lesson.Address).AppendLine()
                          .AppendLine()
                          .AppendLine();
            }

            return strBuilder.ToString();
        }
    }
}