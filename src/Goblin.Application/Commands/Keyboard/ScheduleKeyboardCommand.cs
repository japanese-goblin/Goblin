using System;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Commands.Keyboard
{
    public class ScheduleKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "scheduleKeyboard";

        public Task<IResult> Execute(Message msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                var text = "Для получения расписания установите группу (например - 'установить группу 351919').";
                return Task.FromResult<IResult>(new FailedResult(text));
            }
            
            const string defaultFormat = "dd.MM.yyyy";
            var startDate = DateTime.Now;

            var kb = new KeyboardBuilder(true);
            kb.AddButton($"На сегодня ({startDate:dd.MM - dddd})", startDate.ToString(defaultFormat),
                         KeyboardButtonColor.Primary, "schedule");
            kb.AddLine();
            kb.AddButton($"На завтра ({startDate.AddDays(1):dd.MM - dddd})", startDate.AddDays(1).ToString(defaultFormat),
                         KeyboardButtonColor.Primary, "schedule");
            kb.AddLine();

            startDate = startDate.AddDays(1);
            for(var i = 1; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                kb.AddButton($"На {date:dd.MM (dddd)}", date.ToString(defaultFormat),
                             KeyboardButtonColor.Primary, "schedule");
                if(i % 2 == 0)
                {
                    kb.AddLine();
                }
            }
            kb.AddReturnToMenuButton(false);

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день",
                Keyboard = kb.Build()
            });
        }
    }
}