using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using VkNet.Model;

namespace Goblin.Application.MergedCommands
{
    public class ExamsCommand : IKeyboardCommand, ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "экзамены", "экзы" };
        
        public string Trigger => "exams";
        
        private readonly NarfuApi _api;

        public ExamsCommand(NarfuApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(Message msg, BotUser user = null)
        {
            if(user.NarfuGroup == 0)
            {
                return new FailedResult("Для получения экзаменов сначала необходимо установить группу.");
            }
            
            try
            {
                var lessons = await _api.Students.GetExams(user.NarfuGroup);
                return new SuccessfulResult
                {
                    Message = lessons.ToString()
                };
            }
            catch(FlurlHttpException ex)
            {
                return new FailedResult($"Невозможно получить экзамены с сайта. Попробуйте позже. (Код ошибки - {ex.Call.HttpStatus})");
            }
            catch
            {
                return new FailedResult("Непредвиденная ошибка получения экзаменов с сайта. Попробуйте позже.");
            }
        }
    }
}