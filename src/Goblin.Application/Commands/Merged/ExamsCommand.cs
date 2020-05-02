using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using VkNet.Model;

namespace Goblin.Application.Commands.Merged
{
    public class ExamsCommand : IKeyboardCommand, ITextCommand
    {
        public string Trigger => "exams";

        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "экзамены", "экзы" };

        private readonly NarfuApi _api;

        public ExamsCommand(NarfuApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult(DefaultErrors.GroupNotSet);
            }

            return await _api.Students.GetExamsWithResult(user.NarfuGroup);
        }
    }
}