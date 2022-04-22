using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.ICalParser;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Serilog;

namespace Goblin.Narfu.Schedule;

public class StudentsSchedule : IStudentsSchedule
{
    private readonly HttpClient _client;
    private Group[] Groups { get; }
    private readonly ILogger _logger;

    public StudentsSchedule(string groupsLink, HttpClient client)
    {
        _client = client;
        Groups = _client.GetFromJsonAsync<Group[]>(groupsLink)
                        .GetAwaiter()
                        .GetResult();
        _logger = Log.ForContext<StudentsSchedule>();
    }

    public async Task<IEnumerable<Lesson>> GetSchedule(int realGroupId, DateTime? date = default)
    {
        try
        {
            date ??= DateTime.Today;
            _logger.Debug("Получение расписания для группы {RealGroupId}", realGroupId);
            var siteGroupId = GetGroupByRealId(realGroupId).SiteId;
            var response = await _client.GetStringAsync($"/?icalendar&oid={siteGroupId}&cod={realGroupId}&from={date.Value:dd.MM.yyyy}");
            _logger.Debug("Расписание получено");
            return GetCalendarLessons(response).ToList();
        }
        catch(HttpRequestException)
        {
            var siteGroupId = GetGroupByRealId(realGroupId).SiteId;
            var response = await _client.GetStreamAsync($"/?timetable&group={siteGroupId}");
            _logger.Debug("Расписание получено");
            var allLessonsFromHtml = HtmlParser.GetAllLessonsFromHtml(response);
            return allLessonsFromHtml.Where(x => x.StartTime.Date >= date.Value.Date).ToList();
        }
    }

    public async Task<ExamsViewModel> GetExams(int realGroupId)
    {
        _logger.Debug("Получение списка экзаменов для группы {0}", realGroupId);
        var schedule = await GetSchedule(realGroupId);
        var exams = schedule.Where(x => x.IsExam);
        _logger.Debug("Список экзаменов получен");

        return new ExamsViewModel(exams, DateTime.Today);
    }

    public async Task<LessonsViewModel> GetScheduleAtDate(int realGroupId, DateTime date)
    {
        _logger.Debug("Получение расписания для группы {0} на {1:dd.MM.yyyy}", realGroupId, date);
        var lessons = await GetSchedule(realGroupId);
        return new LessonsViewModel(lessons.Where(x => x.StartTime.Date == date.Date), date);
    }

    public bool IsCorrectGroup(int realGroupId)
    {
        return Groups.Any(x => x.RealId == realGroupId);
    }

    public Group GetGroupByRealId(int realGroupId)
    {
        return Groups.FirstOrDefault(x => x.RealId == realGroupId);
    }

    public string GenerateScheduleLink(int realGroupId, bool isWebCal = false)
    {
        const string url = "ruz.narfu.ru/?icalendar";

        var protocol = isWebCal ? "webcal" : "https";

        var siteGroupId = GetGroupByRealId(realGroupId).SiteId;

        var todayDate = DateTime.Today.ToString("dd.MM.yyyy");

        return $"{protocol}://{url}&oid={siteGroupId}&cod={realGroupId}&from={todayDate}";
    }

    public IEnumerable<Lesson> GetCalendarLessons(string response)
    {
        var calendar = new Calendar(response);

        var events = calendar.Events
                             .Distinct()
                             .OrderBy(x => x.DtStart);

        return events.Select(ev =>
        {
            var description = ev.Description.Split("\\n");
            var address = ev.Location.Split('/');

            if (!int.TryParse(description[0][0].ToString(), out var number))
            {
                number = 1; //в расписании бывают пары, у которых нет номера: п (11:46-11:59)
            }
                
            var lesson = new Lesson
            {
                Id = ev.Uid,
                Address = address[0],
                Auditory = address[1],
                Number = number,
                Groups = description[1].Substring(3),
                Name = ev.Summary.Replace(".", ". "),
                Type = description[3],
                Teacher = description[4],
                StartTime = ev.DtStart,
                EndTime = ev.DtEnd,
                StartEndTime = $"{ev.DtStart:HH:mm} - {ev.DtEnd:HH:mm}"
            };

            if(description.Length <= 6)
            {
                return lesson;
            }

            var possiblyLink = description[6];
            if(Uri.TryCreate(possiblyLink, UriKind.Absolute, out _))
            {
                lesson.Link = possiblyLink;
            }

            return lesson;
        }).Distinct();
    }
}