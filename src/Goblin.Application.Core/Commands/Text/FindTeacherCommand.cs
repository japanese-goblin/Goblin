using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Serilog;

namespace Goblin.Application.Core.Commands.Text
{
    public class FindTeacherCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "препод" };
        private readonly NarfuApi _narfuApi;

        public FindTeacherCommand(NarfuApi narfuApi)
        {
            _narfuApi = narfuApi;
        }

        public async Task<IResult> Execute(IMessage msg, BotUser user)
        {
            var teacherName = string.Join(' ', msg.MessageParams);
            if(string.IsNullOrWhiteSpace(teacherName))
            {
                return new FailedResult("Укажите имя и фамилию преподавателя.");
            }

            try
            {
                var findResult = await _narfuApi.Teachers.FindByName(teacherName);
                if(!findResult.Any())
                {
                    return new FailedResult("Преподаватель с такими данными не найден.");
                }

                if(findResult.Length > 9)
                {
                    return new FailedResult("Найдено слишком много преподавателей. Укажите более точные данные.");
                }

                var keyboard = new CoreKeyboard
                {
                    IsInline = true
                };
                foreach(var teacher in findResult)
                {
                    keyboard.AddButton(teacher.Name, CoreKeyboardButtonColor.Primary,
                                       "teacherSchedule", teacher.Id.ToString());
                    keyboard.AddLine();
                }

                keyboard.AddReturnToMenuButton(false);
                return new SuccessfulResult
                {
                    Message = "Выберите преподавателя из списка:",
                    Keyboard = keyboard
                };
            }
            catch(FlurlHttpException)
            {
                return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<FindTeacherCommand>().Fatal(ex, "Ошибка при поиске преподавателя");
                return new FailedResult(DefaultErrors.NarfuUnexpectedError);
            }
        }
    }
}