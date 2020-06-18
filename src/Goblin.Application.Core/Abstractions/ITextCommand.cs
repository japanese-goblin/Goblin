using System.Threading.Tasks;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Abstractions
{
    public interface ITextCommand
    {
        bool IsAdminCommand { get; }

        string[] Aliases { get; }

        Task<IResult> Execute(IMessage msg, BotUser user);
    }
}