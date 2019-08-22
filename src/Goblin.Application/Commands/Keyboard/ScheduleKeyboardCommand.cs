using System;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
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
            const string DefaultFormat = "dd.MM.yyyy";
            var date = DateTime.Now;

            var kb = new KeyboardBuilder(true);
            kb.AddButton("На сегодня", date.ToString(DefaultFormat),
                         KeyboardButtonColor.Primary, "schedule");
            kb.AddLine();
            kb.AddButton("На завтра", date.AddDays(1).ToString(DefaultFormat),
                         KeyboardButtonColor.Primary, "schedule");
            kb.AddReturnToMenuButton();

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день",
                Keyboard = kb.Build()
            });
        }
    }
}