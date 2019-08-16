using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application.KeyboardCommands
{
    public class WeatherNowCommand : IKeyboardCommand
    {
        public string Trigger => "weatherNow";
        
        public Task<IResult> Execute(Message msg, BotUser user = null)
        {
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Погода на данный момент"
            });
        }
    }
}