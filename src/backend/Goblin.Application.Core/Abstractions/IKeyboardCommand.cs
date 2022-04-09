using System.Threading.Tasks;
using Goblin.Application.Core.Models;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core.Abstractions;

public interface IKeyboardCommand
{
    string Trigger { get; }

    Task<IResult> Execute(Message msg, BotUser user);
}