using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
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

            // var scheduleColor = isSchedule
            //                             ? KeyboardButtonColor.Negative
            //                             : KeyboardButtonColor.Positive;
            // var weatherColor = isWeather
            //                            ? KeyboardButtonColor.Negative
            //                            : KeyboardButtonColor.Positive;

            var scheduleText = isSchedule
                                       ? "Отписаться от рассылки расписания"
                                       : "Подписаться на рассылку расписания";
            var weatherText = isWeather
                                      ? "Отписаться от рассылки погоды"
                                      : "Подписаться на рассылку погоды";

            // var kb = new KeyboardBuilder(true);
            // kb.AddButton(scheduleText, "schedule", scheduleColor, "mailing");
            // kb.AddLine();
            // kb.AddButton(weatherText, "weather", weatherColor, "mailing");
            // kb.AddReturnToMenuButton();

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите действие:",

                // Keyboard = kb.Build()
            });
        }
    }
}