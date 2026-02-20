using System.Net.Http.Json;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Logging;

namespace Goblin.Narfu.Schedule;

public class TeachersSchedule : ITeacherSchedule
{
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public TeachersSchedule(HttpClient client, ILogger<TeachersSchedule> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<Lesson>> GetSchedule(int teacherId)
    {
        _logger.LogDebug("Получение списка пар у преподавателя {TeacherId}", teacherId);
        var response = await _client.GetStreamAsync($"?timetable&lecturer={teacherId}");
        _logger.LogDebug("Список получен");
        return HtmlParser.GetAllLessonsFromHtml(response);
    }

    public async Task<TeacherLessonsViewModel> GetLimitedSchedule(int teacherId, int limit = 10)
    {
        var lessons = await GetSchedule(teacherId);
        var selected = lessons.ToArray()
                              .Where(x => x.StartTime.Date >= DateTime.Today).Take(limit);
        return new TeacherLessonsViewModel(selected, DateTime.Now);
    }

    public async Task<Teacher[]> FindByName(string name)
    {
        _logger.LogDebug("Поиск преподавателя {TeacherName}", name);
        var teachers = await _client.GetFromJsonAsync<Teacher[]>($"i/ac.php?term={name}");
        _logger.LogDebug("Поиск завершен");
        return teachers;
    }
}