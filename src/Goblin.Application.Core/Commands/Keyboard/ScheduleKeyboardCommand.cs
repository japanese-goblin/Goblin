using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class ScheduleKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "scheduleKeyboard";
        
        public Task<IResult> Execute(IMessage msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                return Task.FromResult<IResult>(new FailedResult(DefaultErrors.GroupNotSet));
            }

            const string defaultFormat = "dd.MM.yyyy";
            var startDate = DateTime.Now;

            var keyboard = new CoreKeyboard()
            {
                IsInline = true
            };
            keyboard.AddButton($"На сегодня ({startDate:dd.MM - dddd})", CoreKeyboardButtonColor.Primary,
                         "schedule", startDate.ToString(defaultFormat));
            keyboard.AddLine();
            
            startDate = startDate.AddDays(1);
            keyboard.AddButton($"На завтра ({startDate:dd.MM - dddd})", CoreKeyboardButtonColor.Primary,
                               "schedule", startDate.ToString(defaultFormat));
            keyboard.AddLine();
            
            for(var i = 1; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                keyboard.AddButton($"На {date:dd.MM (dddd)}", CoreKeyboardButtonColor.Primary,
                             "schedule", date.ToString(defaultFormat));
                if(i % 2 == 0)
                {
                    keyboard.AddLine();
                }
            }
            
            keyboard.AddReturnToMenuButton(false);

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день",
                Keyboard = keyboard
            });
        }
    }
}