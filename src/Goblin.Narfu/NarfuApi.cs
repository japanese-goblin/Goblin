using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Schedule;
using Goblin.Narfu.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Goblin.Narfu;

public class NarfuApi : INarfuApi
{
    public ITeacherSchedule Teachers { get; }
    public IStudentsSchedule Students { get; }

    public NarfuApi(IHttpClientFactory httpClientFactory,
                    IOptions<NarfuApiOptions> optionsAccessor,
                    ILogger<TeachersSchedule> teacherScheduleLogger,
                    ILogger<StudentsSchedule> studentsScheduleLogger)
    {
        var client = httpClientFactory.CreateClient(Defaults.HttpClientName);
        Teachers = new TeachersSchedule(client, teacherScheduleLogger);
        Students = new StudentsSchedule(optionsAccessor.Value.NarfuGroupsLink, client, studentsScheduleLogger);
    }
}