using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels;

public class ExamsViewModel : LessonsViewModel
{
    public ExamsViewModel(IEnumerable<Lesson> lessons, DateTime date) : base(lessons, date)
    {
    }

    public override string ToString()
    {
        var exams = Lessons.Where(x => x.StartTime.Date > Date.Date).ToArray();
        if(!exams.Any())
        {
            return "На данный момент список экзаменов отсутствует";
        }

        var strBuilder = new StringBuilder();
        var grouped = exams.GroupBy(x => x.Name);

        foreach(var group in grouped)
        {
            var first = group.First();
            var last = group.Last();

            strBuilder.AppendFormat("{0:D}:", first.StartTime).AppendLine();

            strBuilder.AppendFormat("{0:HH:mm}-{1:HH:mm} - {2} [{3}] ({4})",
                                    first.StartTime, last.EndTime, first.Name, first.Type, first.Teacher)
                      .AppendLine()
                      .AppendFormat("У группы {0}", first.Groups).AppendLine()
                      .AppendFormat("В аудитории {0} ({1})", first.Auditory, first.Address).AppendLine();

            strBuilder.AppendLine();
        }

        return strBuilder.ToString();
    }
}