using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels
{
    public class LessonsViewModel
    {
        protected readonly DateTime _date;
        public IEnumerable<Lesson> Lessons { get; }

        public LessonsViewModel(IEnumerable<Lesson> lessons, DateTime date)
        {
            _date = date;
            Lessons = lessons;
        }

        public override string ToString()
        {
            if(!Lessons.Any())
            {
                return $"На {_date:dd.MM (dddd)} расписание отсутствует!";
            }
            
            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Расписание на {0:dd.MM (dddd)}:", _date).AppendLine();

            foreach(var lesson in Lessons.Where(x => x.StartTime.Date == _date.Date))
            {
                strBuilder.AppendFormat("{0}) {1} - {2} [{3}]", lesson.Number,
                                        lesson.StartEndTime, lesson.Name, lesson.Type)
                          .AppendLine();
                
                if(lesson.Groups != null)
                {
                    strBuilder.AppendFormat("У {0}", lesson.Groups).AppendLine();
                }
                
                strBuilder.AppendFormat("В {0} ({1})", lesson.Auditory, lesson.Address).AppendLine()
                          .AppendLine();
            }

            return strBuilder.ToString();
        }
    }
}