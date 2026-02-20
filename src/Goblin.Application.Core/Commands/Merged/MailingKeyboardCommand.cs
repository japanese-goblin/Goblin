using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Merged;

public class MailingKeyboardCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "mailingKeyboard";

    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "рассылка" };

    public Task<IResult> Execute(Message msg, BotUser user)
    {
        return Task.FromResult<IResult>(new SuccessfulResult
        {
            Message = "Выберите действие:",
            Keyboard = DefaultKeyboards.GetMailingKeyboard(user)
        });
    }
}