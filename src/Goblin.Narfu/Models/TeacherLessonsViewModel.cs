using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goblin.Narfu.Models
{
    public class TeacherLessonsViewModel : LessonsViewModel
    {
        public TeacherLessonsViewModel(IEnumerable<Lesson> lessons, DateTime date) : base(lessons, date)
        {
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            foreach(var group in Lessons.Where(x => x.StartTime.DayOfYear >= _date.DayOfYear)
                                        .GroupBy(x => x.StartTime.Date))
            {
                strBuilder.AppendFormat("{0:D}", group.Key).AppendLine();
                foreach(var lesson in group)
                {
                    strBuilder.AppendFormat("{0:t} - {1} [{2}]",
                                            lesson.StartTime, lesson.Name, lesson.Type)
                              .AppendLine()
                              .AppendFormat("У группы {0}", lesson.Groups).AppendLine()
                              .AppendFormat("В {0} ({1})", lesson.Auditory, lesson.Address).AppendLine()
                              .AppendLine()
                              .AppendLine();
                }
            }

            return strBuilder.ToString();
        }
    }
}