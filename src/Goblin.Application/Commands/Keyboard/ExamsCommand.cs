using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using VkNet.Model;

namespace Goblin.Application.Commands.Keyboard
{
    public class ExamsCommand : IKeyboardCommand
    {
        public string Trigger => "exams";

        private readonly NarfuApi _api;

        public ExamsCommand(NarfuApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult("Для получения экзаменов установите группу (нужно написать следующее - установить группу 123456).");
            }

            return await _api.Students.GetExamsWithResult(user.NarfuGroup);
        }
    }
}