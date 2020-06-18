using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class ScheduleKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "scheduleKeyboard";

        //TODO:
        public Task<IResult> Execute(IMessage msg, BotUser user)
        {
            if(user.NarfuGroup == 0)
            {
                return Task.FromResult<IResult>(new FailedResult(DefaultErrors.GroupNotSet));
            }

            const string defaultFormat = "dd.MM.yyyy";
            var startDate = DateTime.Now;

            // var kb = new KeyboardBuilder(true);
            // kb.AddButton($"На сегодня ({startDate:dd.MM - dddd})", startDate.ToString(defaultFormat),
            //              KeyboardButtonColor.Primary, "schedule");
            // kb.AddLine();
            //
            // startDate = startDate.AddDays(1);
            // kb.AddButton($"На завтра ({startDate:dd.MM - dddd})", startDate.ToString(defaultFormat),
            //              KeyboardButtonColor.Primary, "schedule");
            // kb.AddLine();
            //
            // for(var i = 1; i < 7; i++)
            // {
            //     var date = startDate.AddDays(i);
            //     kb.AddButton($"На {date:dd.MM (dddd)}", date.ToString(defaultFormat),
            //                  KeyboardButtonColor.Primary, "schedule");
            //     if(i % 2 == 0)
            //     {
            //         kb.AddLine();
            //     }
            // }
            //
            // kb.AddReturnToMenuButton(false);

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день",

                // Keyboard = kb.Build()
            });
        }
    }
}