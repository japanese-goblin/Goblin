using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Text
{
    public class RemoveKeyboardCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "куб" };

        public Task<IResult> Execute(IMessage msg, BotUser user)
        { 
            var kb = new CoreKeyboard();
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Окей",
                Keyboard = kb
            });
        }
    }
}