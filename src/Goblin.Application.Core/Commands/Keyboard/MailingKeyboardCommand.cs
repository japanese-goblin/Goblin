using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class MailingKeyboardCommand : IKeyboardCommand
    {
        public string Trigger => "mailingKeyboard";

        public Task<IResult> Execute(IMessage msg, BotUser user)
        {
            //TODO: here
            var isSchedule = user.SubscribeInfo.IsSchedule;
            var isWeather = user.SubscribeInfo.IsWeather;

            var scheduleColor = isSchedule
                                        ? CoreKeyboardButtonColor.Negative
                                        : CoreKeyboardButtonColor.Positive;
            var weatherColor = isWeather
                                       ? CoreKeyboardButtonColor.Negative
                                       : CoreKeyboardButtonColor.Positive;

            var scheduleText = isSchedule
                                       ? "Отписаться от рассылки расписания"
                                       : "Подписаться на рассылку расписания";
            var weatherText = isWeather
                                      ? "Отписаться от рассылки погоды"
                                      : "Подписаться на рассылку погоды";

            var kb = new CoreKeyboard();
            kb.AddButton(scheduleText, scheduleColor, "schedule", "mailing");
            kb.AddLine();
            kb.AddButton(weatherText, weatherColor, "weather", "mailing");
            kb.AddReturnToMenuButton();

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите действие:",
                Keyboard = kb
            });
        }
    }
}