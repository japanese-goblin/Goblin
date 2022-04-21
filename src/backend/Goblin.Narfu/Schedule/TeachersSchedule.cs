using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Serilog;

namespace Goblin.Narfu.Schedule;

public class TeachersSchedule : ITeacherSchedule
{
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public TeachersSchedule(HttpClient client)
    {
        _client = client;
        _logger = Log.ForContext<TeachersSchedule>();
    }

    public async Task<IEnumerable<Lesson>> GetSchedule(int teacherId)
    {
        _logger.Debug("Получение списка пар у преподавателя {TeacherId}", teacherId);
        var response = await _client.GetStreamAsync($"?timetable&lecturer={teacherId}");
        _logger.Debug("Список получен");
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
        _logger.Debug("Поиск преподавателя {TeacherName}", name);
        var teachers = await _client.GetFromJsonAsync<Teacher[]>($"i/ac.php?term={name}");
        _logger.Debug("Поиск завершен");
        return teachers;
    }
}