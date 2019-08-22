using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Commands.Text
{
    public class RemoveKeyboardCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "куб" };
        
        public Task<IResult> Execute(Message msg, BotUser user)
        {
            var kb = new KeyboardBuilder();
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Окей",
                Keyboard = kb.Build()
            });
        }
    }
}