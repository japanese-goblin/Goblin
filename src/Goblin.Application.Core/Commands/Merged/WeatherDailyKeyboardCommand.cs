using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Merged
{
    public class WeatherDailyKeyboardCommand : IKeyboardCommand, ITextCommand
    {
        public string Trigger => "weatherDailyKeyboard";

        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "ежедневная" }; //TODO: lol

        public Task<IResult> Execute(IMessage msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return Task.FromResult<IResult>(new FailedResult(DefaultErrors.CityNotSet));
            }

            const string defaultFormat = "dd.MM.yyyy";
            var date = DateTime.Now;
            var kb = new CoreKeyboard
            {
                IsInline = true
            };
            kb.AddButton("На сегодня", CoreKeyboardButtonColor.Primary,
                         "weatherDaily", date.ToString(defaultFormat));
            kb.AddLine();
            date = date.AddDays(1);
            kb.AddButton("На завтра", CoreKeyboardButtonColor.Primary,
                         "weatherDaily", date.ToString(defaultFormat));
            kb.AddReturnToMenuButton();

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день для получения погоды:",
                Keyboard = kb
            });
        }
    }
}