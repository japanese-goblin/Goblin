using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Merged;

public class HelpCommand : IKeyboardCommand, ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "помоги", "справка", "помощь", "команды" };

    public string Trigger => "help";

    public Task<IResult> Execute(Message msg, BotUser user)
    {
        const string guideLink = "https://vk.com/@-146048760-commands";
        return Task.FromResult<IResult>(new SuccessfulResult
        {
            Message = $"Список всех доступных команд здесь: {guideLink}"
        });
    }
}