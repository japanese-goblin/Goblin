using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels
{
    public class LessonsViewModel
    {
        public IEnumerable<Lesson> Lessons { get; }
        protected readonly DateTime Date;

        public LessonsViewModel(IEnumerable<Lesson> lessons, DateTime date)
        {
            Date = date;
            Lessons = lessons;
        }

        public override string ToString()
        {
            if(!Lessons.Any())
            {
                return $"На {Date:dd.MM (dddd)} расписание отсутствует!";
            }

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Расписание на {0:dd.MM (dddd)}:", Date).AppendLine();

            foreach(var lesson in Lessons.Where(x => x.StartTime.Date == Date.Date))
            {
                strBuilder.AppendFormat("{0}) {1} - {2} ({3}) [{4}]", lesson.Number,
                                        lesson.StartEndTime, lesson.Name, lesson.Teacher, lesson.Type)
                          .AppendLine();

                if(lesson.Groups != null)
                {
                    strBuilder.AppendFormat("У группы {0}", lesson.Groups).AppendLine();
                }

                strBuilder.AppendFormat("В ауд. {0} ({1})", lesson.Auditory, lesson.Address).AppendLine()
                          .AppendLine();
            }

            return strBuilder.ToString();
        }
    }
}