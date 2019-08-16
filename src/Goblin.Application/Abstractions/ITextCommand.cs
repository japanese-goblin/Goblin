using System.Threading.Tasks;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application.Abstractions
{
    public interface ITextCommand
    {
        string Name { get; }
        string Description { get; }
        string Usage { get; }
        bool IsAdminCommand { get; }

        string[] Aliases { get; }

        Task<IResult> Execute(Message msg, BotUser user = null);
    }
}