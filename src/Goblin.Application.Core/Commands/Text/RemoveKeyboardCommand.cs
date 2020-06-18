using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
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
            //TODO: keyboard
            // var kb = new KeyboardBuilder();
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Окей",

                // Keyboard = kb.Build()
            });
        }
    }
}