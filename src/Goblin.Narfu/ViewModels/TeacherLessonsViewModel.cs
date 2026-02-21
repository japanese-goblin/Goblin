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
            strBuilder.Append($"{group.Key:D}").AppendLine();
            foreach(var lesson in group)
            {
                strBuilder.Append($"{lesson.Number}) {lesson.StartEndTime} - {lesson.Name} [{lesson.Type}]")
                          .AppendLine()
                          .Append($"У группы {lesson.Groups}").AppendLine()
                          .Append($"В {lesson.Auditory} ({lesson.Address})").AppendLine();
            }

            strBuilder.AppendLine();
        }

        return strBuilder.ToString();
    }
}