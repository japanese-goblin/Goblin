using System;
using System.Net.Http;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Goblin.Narfu.Abstractions;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Commands.Merged;

public class ExamsCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "exams";

    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "экзамены", "экзы" };

    private readonly INarfuApi _api;
    private readonly ILogger<ExamsCommand> _logger;

    public ExamsCommand(INarfuApi api, ILogger<ExamsCommand> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<IResult> Execute(Message msg, BotUser user)
    {
        if(user.NarfuGroup == 0)
        {
            return new FailedResult(DefaultErrors.GroupNotSet);
        }

        try
        {
            var lessons = await _api.Students.GetExams(user.NarfuGroup);
            var str = lessons.ToString();
            if(str.Length > 4096)
            {
                str = $"{str[..4000]}...\n\nПолный список экзаменов можете посмотреть на сайте";
            }
            return new SuccessfulResult
            {
                Message = str
            };
        }
        catch(Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении расписания на день");
            return new FailedResult(DefaultErrors.NarfuUnexpectedError);
        }
    }
}