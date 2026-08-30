using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Schedule;
using Microsoft.Extensions.Logging;

namespace Goblin.Narfu;

public class NarfuApi : INarfuApi
{
    public ITeacherSchedule Teachers { get; }
    public IStudentsSchedule Students { get; }

    public NarfuApi(IHttpClientFactory httpClientFactory,
                    INarfuGroupsCache groupsCache,
                    ILogger<TeachersSchedule> teacherScheduleLogger,
                    ILogger<StudentsSchedule> studentsScheduleLogger)
    {
        var client = httpClientFactory.CreateClient(Defaults.HttpClientName);
        Teachers = new TeachersSchedule(client, teacherScheduleLogger);
        Students = new StudentsSchedule(groupsCache, client, studentsScheduleLogger);
    }
}
