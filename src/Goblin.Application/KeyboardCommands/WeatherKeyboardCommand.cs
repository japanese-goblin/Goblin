using System;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace Goblin.Application.KeyboardCommands
{
    public class WeatherKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "weatherKeyboard";
        
        public Task<IResult> Execute(Message msg, BotUser user)
        {
            const string DefaultFormat = "dd.MM.yyyy";
            var date = DateTime.Now;
            var kb = new KeyboardBuilder(true);
            kb.AddButton("На сегодня", "weather",
                         KeyboardButtonColor.Primary, date.ToString(DefaultFormat));
            kb.AddLine();
            kb.AddButton("На завтра", "weather",
                         KeyboardButtonColor.Primary, date.AddDays(1).ToString(DefaultFormat));
            kb.AddReturnToMenuButton();

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день для получения погоды:",
                Keyboard = kb.Build()
            });
        }
    }
}