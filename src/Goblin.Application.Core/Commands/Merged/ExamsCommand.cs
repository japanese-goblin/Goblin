using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;
using Goblin.Narfu;
using Goblin.Narfu.Abstractions;
using Serilog;

namespace Goblin.Application.Core.Commands.Merged
{
    public class ExamsCommand : IKeyboardCommand, ITextCommand
    {
        public string Trigger => "exams";

        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "экзамены", "экзы" };

        private readonly INarfuApi _api;

        public ExamsCommand(INarfuApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult(DefaultErrors.GroupNotSet);
            }

            try
            {
                var lessons = await _api.Students.GetExams(user.NarfuGroup);
                var str = lessons.ToString();
                if(str.Length > 4096)
                {
                    str = $"{str[..4000]}...\n\nПолный список экзаменов можете посмотреть на сайте";
                }
                return new SuccessfulResult
                {
                    Message = str
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