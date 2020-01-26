using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application.Commands.Keyboard
{
    public class GetRemindsCommand : IKeyboardCommand
    {
        public string Trigger => "reminds";

        public Task<IResult> Execute(Message msg, BotUser user)
        {
            if(user.Reminds.Count == 0)
            {
                return Task.FromResult<IResult>(new SuccessfulResult
                {
                    Message = "У Вас нет ни одного добавленного напоминания."
                });
            }

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("Список напоминаний:");
            foreach(var userRemind in user.Reminds.OrderBy(x => x.Date))
            {
                strBuilder.AppendFormat("{0:dd.MM.yyyy HH:mm} - {1}", userRemind.Date, userRemind.Text)
                          .AppendLine();
            }

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = strBuilder.ToString()
            });
        }
    }
}