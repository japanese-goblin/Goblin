using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core.Commands.Merged;

public class ScheduleKeyboardCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "scheduleKeyboard";

    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "расписание" };

    public Task<IResult> Execute(Message msg, BotUser user)
    {
        if(user.NarfuGroup == 0)
        {
            return Task.FromResult<IResult>(new FailedResult(DefaultErrors.GroupNotSet));
        }

        return Task.FromResult<IResult>(new SuccessfulResult
        {
            Message = "Выберите день",
            Keyboard = DefaultKeyboards.GetScheduleKeyboard()
        });
    }
}