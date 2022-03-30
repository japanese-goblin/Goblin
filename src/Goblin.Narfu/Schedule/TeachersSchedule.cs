using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Serilog;

namespace Goblin.Narfu.Schedule;

public class TeachersSchedule : ITeacherSchedule
{
    private readonly ILogger _logger;

    public TeachersSchedule()
    {
        _logger = Log.ForContext<TeachersSchedule>();
    }

    public async Task<IEnumerable<Lesson>> GetSchedule(int teacherId)
    {
        _logger.Debug("Получение списка пар у преподавателя {0}", teacherId);
        var response = await RequestBuilder.Create()
                                           .SetQueryParam("timetable")
                                           .SetQueryParam("lecturer", teacherId)
                                           .GetStreamAsync();
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
        _logger.Debug("Поиск преподавателя {0}", name);
        var teachers = await RequestBuilder.Create()
                                           .AppendPathSegments("i", "ac.php")
                                           .SetQueryParam("term", name)
                                           .GetJsonAsync<Teacher[]>();
        _logger.Debug("Поиск завершен");

        return teachers;
    }
}