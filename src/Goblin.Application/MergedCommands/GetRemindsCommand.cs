using System.Text;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application.MergedCommands
{
    public class GetRemindsCommand : IKeyboardCommand, ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "напоминания" };
        
        public string Trigger => "reminds";

        public Task<IResult> Execute(Message msg, BotUser user = null)
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
            foreach(var userRemind in user.Reminds)
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