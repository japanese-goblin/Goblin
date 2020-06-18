using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class WeatherDailyKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "weatherDailyKeyboard";

        //TODO:
        public Task<IResult> Execute(IMessage msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return Task.FromResult<IResult>(new FailedResult(DefaultErrors.CityNotSet));
            }

            // const string defaultFormat = "dd.MM.yyyy";
            // var date = DateTime.Now;
            // var kb = new KeyboardBuilder(true);
            // kb.AddButton("На сегодня", date.ToString(defaultFormat),
            //              KeyboardButtonColor.Primary, "weatherDaily");
            // kb.AddLine();
            // kb.AddButton("На завтра", date.AddDays(1).ToString(defaultFormat),
            //              KeyboardButtonColor.Primary, "weatherDaily");
            // kb.AddReturnToMenuButton();

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите день для получения погоды:",

                // Keyboard = kb.Build()
            });
        }
    }
}