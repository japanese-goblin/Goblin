using System.Threading.Tasks;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application.Abstractions
{
    public interface ITextCommand
    {
        bool IsAdminCommand { get; }

        string[] Aliases { get; }

        Task<IResult> Execute(Message msg, BotUser user);
    }
}