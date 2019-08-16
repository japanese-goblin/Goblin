using System;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace Goblin.Application.KeyboardCommands
{
    public class ScheduleKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "scheduleKeyboard";
        
        public Task<IResult> Execute(Message msg, BotUser user = null)
        {
            const string DefaultFormat = "dd.MM.yyyy";
            var date = DateTime.Now;
            
            var kb = new KeyboardBuilder(true);
            kb.AddButton("На сегодня", "schedule",
                         KeyboardButtonColor.Primary, date.ToString(DefaultFormat));
            kb.AddLine();
            kb.AddButton("На завтра", "schedule",
                         KeyboardButtonColor.Primary, date.AddDays(1).ToString(DefaultFormat));
            kb.AddLine();

            kb.AddButton("Вернуться в главное меню", "command",
                         KeyboardButtonColor.Default, "start");
            
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день",
                Keyboard = kb.Build()
            });
        }
    }
}