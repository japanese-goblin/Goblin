using System.Text;
using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels;

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
        strBuilder.Append($"Расписание на {Date:dd.MM (dddd)}:").AppendLine();

        foreach(var lesson in Lessons.Where(x => x.StartTime.Date == Date.Date))
        {
            strBuilder.Append($"{lesson.Number}) {lesson.StartEndTime} - {lesson.Name} ({lesson.Teacher}) [{lesson.Type}]")
                      .AppendLine();

            if(!string.IsNullOrWhiteSpace(lesson.Groups))
            {
                strBuilder.Append($"У группы {lesson.Groups}").AppendLine();
            }

            strBuilder.Append($"В ауд. {lesson.Auditory} ({lesson.Address})").AppendLine()
                      .AppendLine();
        }

        return strBuilder.ToString();
    }
}