using System.Net.Http.Json;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Logging;

namespace Goblin.Narfu.Schedule;

public class StudentsSchedule : IStudentsSchedule
{
    private readonly HttpClient _client;
    private Group[] Groups { get; }
    private readonly ILogger _logger;

    public StudentsSchedule(string groupsLink, HttpClient client, ILogger<StudentsSchedule> logger)
    {
        _client = client;
        Groups = _client.GetFromJsonAsync<Group[]>(groupsLink)
                        .GetAwaiter()
                        .GetResult();
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Lesson>> GetSchedule(int realGroupId, DateTime? date = null)
    {
        date ??= DateTime.Today;
        var siteGroupId = GetGroupByRealId(realGroupId).SiteId;

        try
        {
            _logger.LogDebug("Получение расписания для группы {GroupId}", realGroupId);
            var response = await _client.GetStringAsync($"/?icalendar&oid={siteGroupId}&cod={realGroupId}&from={date.Value:dd.MM.yyyy}");
            _logger.LogDebug("Расписание получено");
            return GetCalendarLessons(response);
        }
        catch(HttpRequestException)
        {
            var response = await _client.GetStreamAsync($"/?timetable&group={siteGroupId}");
            _logger.LogDebug("Расписание получено");
            var allLessonsFromHtml = HtmlParser.GetAllLessonsFromHtml(response);
            return allLessonsFromHtml.Where(x => x.StartTime.Date >= date.Value.Date).ToList();
        }
    }

    public async Task<ExamsViewModel> GetExams(int realGroupId)
    {
        _logger.LogDebug("Получение списка экзаменов для группы {GroupId}", realGroupId);
        var schedule = await GetSchedule(realGroupId);
        var exams = schedule.Where(x => x.IsExam);
        _logger.LogDebug("Список экзаменов получен");

        return new ExamsViewModel(exams, DateTime.Today);
    }

    public async Task<LessonsViewModel> GetScheduleAtDate(int realGroupId, DateTime date)
    {
        _logger.LogDebug("Получение расписания для группы {GroupId} на {ScheduleDate:dd.MM.yyyy}", realGroupId, date);
        var lessons = await GetSchedule(realGroupId);
        return new LessonsViewModel(lessons.Where(x => x.StartTime.Date == date.Date), date);
    }

    public Group? GetGroupByRealId(int realGroupId)
    {
        return Groups.FirstOrDefault(x => x.RealId == realGroupId);
    }

    // public string GenerateScheduleLink(int realGroupId, bool isWebCal = false)
    // {
    //     const string url = "ruz.narfu.ru/?icalendar";
    //
    //     var protocol = isWebCal ? "webcal" : "https";
    //
    //     var siteGroupId = GetGroupByRealId(realGroupId).SiteId;
    //
    //     var todayDate = DateTime.Today.ToString("dd.MM.yyyy");
    //
    //     return $"{protocol}://{url}&oid={siteGroupId}&cod={realGroupId}&from={todayDate}";
    // }

    private static List<Lesson> GetCalendarLessons(string response)
    {
        var calendar = Ical.Net.Calendar.Load(response);
        if(calendar is null)
        {
            return [];
        }

        var result = new List<Lesson>(calendar.Events.Count);

        var calendarEvents = calendar.Events
                             .DistinctBy(p => p.Uid)
                             .OrderBy(p => p.DtStart);
        foreach(var calendarEvent in calendarEvents)
        {
            if(calendarEvent.DtStart is null ||
               calendarEvent.DtEnd is null ||
               calendarEvent.Uid is null ||
               calendarEvent.Description is null ||
               calendarEvent.Location is null ||
               calendarEvent.Summary is null)
            {
                continue;
            }
            var description = calendarEvent.Description.Split('\n');
            var address = calendarEvent.Location.Split('/');

            if(!int.TryParse(description[0][0].ToString(), out var number))
            {
                number = 1; //в расписании бывают пары, у которых нет номера: п (11:46-11:59)
            }

            var lesson = new Lesson
            {
                Id = calendarEvent.Uid,
                Address = address[0],
                Auditory = address[1],
                Number = number,
                Groups = description[1][3..],
                Name = calendarEvent.Summary.Replace(".", ". "),
                Type = description[3],
                Teacher = description[4],
                StartTime = calendarEvent.DtStart.Value.ToLocalTime(),
                EndTime = calendarEvent.DtEnd.Value.ToLocalTime()
            };
            result.Add(lesson);

            if(description.Length <= 6)
            {
                continue;
            }

            var possiblyLink = description[6];
            if(Uri.TryCreate(possiblyLink, UriKind.Absolute, out _))
            {
                lesson.Link = possiblyLink;
            }
        }

        return result;
    }
}