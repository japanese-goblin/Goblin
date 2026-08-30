using System.Net.Http.Json;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Logging;

namespace Goblin.Narfu.Schedule;

public class TeachersSchedule(HttpClient client, ILogger<TeachersSchedule> logger) : ITeacherSchedule
{
    public async Task<IEnumerable<Lesson>> GetSchedule(int teacherId)
    {
        logger.LogDebug("Получение списка пар у преподавателя {TeacherId}", teacherId);
        var response = await client.GetStreamAsync($"?timetable&lecturer={teacherId}");
        logger.LogDebug("Список получен");
        return HtmlParser.GetAllLessonsFromHtml(response);
    }

    public async Task<TeacherLessonsViewModel> GetLimitedSchedule(int teacherId, int limit = 10)
    {
        var lessons = await GetSchedule(teacherId);
        var selected = lessons.Where(x => x.StartTime.Date >= DateTime.Today)
                              .Take(limit)
                              .ToList();
        return new TeacherLessonsViewModel(selected);
    }

    public async Task<Teacher[]> FindByName(string name)
    {
        logger.LogDebug("Поиск преподавателя {TeacherName}", name);
        var teachers = await client.GetFromJsonAsync<Teacher[]>($"i/ac.php?term={name}");
        logger.LogDebug("Поиск завершен");
        return teachers ?? [];
    }
}