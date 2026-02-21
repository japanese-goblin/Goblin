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
        if(exams.Length == 0)
        {
            return "На данный момент список экзаменов отсутствует";
        }

        var strBuilder = new StringBuilder();
        var grouped = exams.GroupBy(x => x.Name);

        foreach(var group in grouped)
        {
            var first = group.First();
            var last = group.Last();

            strBuilder.Append($"{first.StartTime:D}:").AppendLine();

            strBuilder.Append($"{first.StartTime:HH:mm}-{last.EndTime:HH:mm} - {first.Name} [{first.Type}] ({first.Teacher})")
                      .AppendLine()
                      .Append($"У группы {first.Groups}").AppendLine()
                      .Append($"В аудитории {first.Auditory} ({first.Address})").AppendLine();

            strBuilder.AppendLine();
        }

        return strBuilder.ToString();
    }
}