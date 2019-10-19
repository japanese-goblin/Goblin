using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application.Commands.Merged
{
    public class StartCommand : IKeyboardCommand, ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "старт", "начать" };
        
        public string Trigger => "command";

        public Task<IResult> Execute(Message msg, BotUser user)
        {
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите действие:",
                Keyboard = DefaultKeyboards.GetDefaultKeyboard()
            });
        }
    }
}