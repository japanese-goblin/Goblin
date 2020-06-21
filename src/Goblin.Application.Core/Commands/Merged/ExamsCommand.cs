using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Serilog;

namespace Goblin.Application.Core.Commands.Merged
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

        public async Task<IResult> Execute(IMessage msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult(DefaultErrors.GroupNotSet);
            }

            try
            {
                var lessons = await _api.Students.GetExams(user.NarfuGroup);
                return new SuccessfulResult
                {
                    Message = lessons.ToString()
                };
            }
            catch(FlurlHttpException)
            {
                return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<NarfuApi>().Fatal(ex, "Ошибка при получении экзаменов");
                return new FailedResult(DefaultErrors.NarfuUnexpectedError);
            }
        }
    }
}