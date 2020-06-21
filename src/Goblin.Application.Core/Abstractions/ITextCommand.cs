using System.Threading.Tasks;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core.Abstractions
{
    public interface ITextCommand
    {
        bool IsAdminCommand { get; }

        string[] Aliases { get; }

        Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser;
    }
}