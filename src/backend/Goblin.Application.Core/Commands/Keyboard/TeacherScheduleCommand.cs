using System;
using System.Net.Http;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu.Abstractions;

namespace Goblin.Application.Core.Commands.Keyboard;

public class TeacherScheduleCommand : IKeyboardCommand
{
    public string Trigger => "teacherSchedule";
    private readonly INarfuApi _narfuApi;

    public TeacherScheduleCommand(INarfuApi narfuApi)
    {
        _narfuApi = narfuApi;
    }

    public async Task<IResult> Execute(Message msg, BotUser user)
    {
        var dict = msg.ParsedPayload;
        var isExists = dict.TryGetValue(Trigger, out var idString);
        if(!isExists)
        {
            return new FailedResult("Невозожно получить ID преподавателя.");
        }

        var isCorrectId = int.TryParse(idString, out var id);
        if(!isCorrectId)
        {
            return new FailedResult("Некорректный ID преподавателя.");
        }

        try
        {
            var schedule = await _narfuApi.Teachers.GetLimitedSchedule(id);

            return new SuccessfulResult
            {
                Message = schedule.ToString()
            };
        }
        catch(HttpRequestException)
        {
            return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception)
        {
            return new FailedResult(DefaultErrors.NarfuUnexpectedError);
        }
    }
}