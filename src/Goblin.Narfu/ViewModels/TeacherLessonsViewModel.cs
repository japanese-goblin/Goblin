using System.Text;
using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels;

public class TeacherLessonsViewModel : LessonsViewModel
{
    public TeacherLessonsViewModel(IEnumerable<Lesson> lessons, DateTime date) : base(lessons, date)
    {
    }

    public override string ToString()
    {
        var selected = Lessons.ToArray();
        if(selected.Length == 0)
        {
            return "На данный момент у этого преподавателя нет пар";
        }

        var strBuilder = new StringBuilder();

        foreach(var group in selected.GroupBy(x => x.StartTime.Date))
        {
            strBuilder.AppendFormat("{0:D}", group.Key).AppendLine();
            foreach(var lesson in group)
            {
                strBuilder.AppendFormat("{0}) {1} - {2} [{3}]",
                                        lesson.Number, lesson.StartEndTime, lesson.Name, lesson.Type)
                          .AppendLine()
                          .AppendFormat("У группы {0}", lesson.Groups).AppendLine()
                          .AppendFormat("В {0} ({1})", lesson.Auditory, lesson.Address).AppendLine();
            }

            strBuilder.AppendLine();
        }

        return strBuilder.ToString();
    }
}