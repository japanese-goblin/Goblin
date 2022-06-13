using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Merged;

public class StartCommand : IKeyboardCommand, ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "старт", "начать", "/start" };

    public string Trigger => "command";

    public Task<IResult> Execute(Message msg, BotUser user)
    {
        return Task.FromResult<IResult>(new SuccessfulResult
        {
            Message = "Выберите действие:",
            Keyboard = DefaultKeyboards.GetDefaultKeyboard()
        });
    }
}