using System.Threading.Tasks;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Abstractions
{
    public interface IKeyboardCommand
    {
        string Trigger { get; }

        Task<IResult> Execute(IMessage msg, BotUser user);
    }
}