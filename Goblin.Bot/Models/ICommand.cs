using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Domain.Entities;
using Vk.Models.Messages;

namespace Goblin.Bot.Models
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Usage { get; }
        string[] Aliases { get; }
        CommandCategory Category { get; }
        bool IsAdmin { get; }

        Task<CommandResponse> Execute(Message msg, BotUser user);
        (bool Success, string Text) CanExecute(Message msg, BotUser user);
    }
}