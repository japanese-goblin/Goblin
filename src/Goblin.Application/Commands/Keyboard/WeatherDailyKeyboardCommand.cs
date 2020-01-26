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
    public class WeatherDailyKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "weatherDailyKeyboard";

        public Task<IResult> Execute(Message msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return Task.FromResult<IResult>(new FailedResult(DefaultErrors.CityNotSet));
            }

            const string defaultFormat = "dd.MM.yyyy";
            var date = DateTime.Now;
            var kb = new KeyboardBuilder(true);
            kb.AddButton("На сегодня", date.ToString(defaultFormat),
                         KeyboardButtonColor.Primary, "weatherDaily");
            kb.AddLine();
            kb.AddButton("На завтра", date.AddDays(1).ToString(defaultFormat),
                         KeyboardButtonColor.Primary, "weatherDaily");
            kb.AddReturnToMenuButton();

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день для получения погоды:",
                Keyboard = kb.Build()
            });
        }
    }
}