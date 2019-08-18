using System.Threading.Tasks;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application.Abstractions
{
    public interface IKeyboardCommand
    {
        string Trigger { get; }
        
        Task<IResult> Execute(Message msg, BotUser user);
    }
}