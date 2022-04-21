using FastEndpoints;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.WebApp.Endpoints.Schedule;

public class Get : Endpoint<ScheduleRequest, ScheduleResponse>
{
    private readonly INarfuApi _narfuApi;

    public Get(INarfuApi narfuApi)
    {
        _narfuApi = narfuApi;
    }

    public override void Configure()
    {
        Get("schedule/{groupId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ScheduleRequest req, CancellationToken ct)
    {
        var group = _narfuApi.Students.GetGroupByRealId(req.GroupId);
        if(group is null)
        {
            AddError(x => x.GroupId, $"Группа '{req.GroupId}' не найдена");
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        var lessons = (await _narfuApi.Students.GetSchedule(req.GroupId, req.Date)).ToList();
        await SendAsync(new ScheduleResponse
        {
            IsFromSite = lessons.Any(x => string.IsNullOrEmpty(x.Id)),
            Lessons = MagicWithLessons(lessons),
            GroupId = req.GroupId,
            GroupName = group.Name,
            IcsLink = _narfuApi.Students.GenerateScheduleLink(req.GroupId),
            WebCalLink = _narfuApi.Students.GenerateScheduleLink(req.GroupId, true),
        }, cancellation: ct);
    }
    
    private static DateTime GetStartOfWeek(DateTime dt)
    {
        const DayOfWeek startOfWeek = DayOfWeek.Monday;
        
        var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
    
    // какой ужас.... но работает (TODO)
    private static Dictionary<DateTime, Dictionary<DateTime, IEnumerable<Lesson>>> MagicWithLessons(ICollection<Lesson> lessons)
    {
        if(!lessons.Any())
        {
            return new Dictionary<DateTime, Dictionary<DateTime, IEnumerable<Lesson>>>();
        }

        var first = lessons.First().StartTime.Date;
        var last = lessons.Last().StartTime.Date;
        var dif = (last - first).Days;
        for(var i = 0; i <= dif; i++)
        {
            var day = first.AddDays(i);
            if(day.DayOfWeek == DayOfWeek.Sunday) // по воскресениям пар нет
            {
                continue;
            }

            var lessonsAtDay = lessons.Where(x => x.StartTime.Date == day).ToArray();
            if(lessonsAtDay.Any())
            {
                var max = lessonsAtDay.Max(x => x.Number); // количество пар в этот день
                for(var lessonNumber = 1; lessonNumber <= max; lessonNumber++)
                {
                    var isPreviousBreak = string.IsNullOrWhiteSpace(lessonsAtDay.FirstOrDefault(x => x.Number == lessonNumber - 1)?.Address) && lessonNumber > 1;
                    if(lessonsAtDay.All(x => x.Number != lessonNumber) && !isPreviousBreak) // если нет пары под этим номером 
                    {
                        lessons.Add(new Lesson
                        {
                            StartTime = day,
                            Number = lessonNumber,
                            Name = lessonNumber == 1 ? "Здоровый сон" : "Перерыв"
                        });
                    }
                }
            }
            else
            {
                // добавление пустого дня в расписании
                lessons.Add(new Lesson
                {
                    StartTime = day,
                    Number = 1,
                    Name = "Выходной"
                });
            }
        }

        var dict = lessons.OrderBy(x => x.StartTime.Date)
                          .ThenBy(x => x.Number)
                          .GroupBy(x => GetStartOfWeek(x.StartTime));

        return dict.ToDictionary(x => x.Key,
                                 x => x.GroupBy(y => y.StartTime.Date)
                                       .ToDictionary(y => y.Key, y => y.AsEnumerable()));
    }
}

public class ScheduleRequest
{
    public int GroupId { get; set; }

    [QueryParam]
    public DateTime? Date { get; set; }
}

public class ScheduleResponse
{
    public string GroupName { get; set; }
    public int GroupId { get; set; }
    public Dictionary<DateTime, Dictionary<DateTime, IEnumerable<Lesson>>> Lessons { get; set; }
    public string? WebCalLink { get; set; }
    public string? IcsLink { get; set; }
    public bool IsFromSite { get; set; }
}