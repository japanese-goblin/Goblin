using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Commands.Text
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

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            var teacherName = string.Join(' ', msg.GetCommandParameters());
            if(string.IsNullOrWhiteSpace(teacherName))
            {
                return new FailedResult("Укажите имя и фамилию преподавателя.");
            }

            try
            {
                var findResult = await _narfuApi.Teachers.FindByName(teacherName);
                if(!findResult.Any())
                {
                    return new FailedResult("Преподаватель с такими данным не найден.");
                }

                if(findResult.Length > 9)
                {
                    return new FailedResult("Найдено слишком много преподавателей. Укажите более точные данные.");
                }

                var kb = new KeyboardBuilder(true);
                foreach(var teacher in findResult)
                {
                    kb.AddButton(teacher.Name, teacher.Id.ToString(), KeyboardButtonColor.Primary, "teacherSchedule");
                    kb.AddLine();
                }

                kb.AddReturnToMenuButton(false);
                return new SuccessfulResult
                {
                    Message = "Выберите преподавателя из списка:",
                    Keyboard = kb.Build()
                };
            }
            catch(FlurlHttpException ex)
            {
                return new
                        FailedResult($"Сайт с расписанием недоступен (код ошибки - {ex.Call.HttpStatus}). Попробуйте позже.");
            }
            catch(Exception)
            {
                return new FailedResult("Непредвиденная ошибка получения списка преподавателей. Попробуйте позже.");
            }
        }
    }
}