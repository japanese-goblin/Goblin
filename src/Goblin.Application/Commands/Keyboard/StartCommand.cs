using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Commands.Keyboard
{
    public class StartCommand : IKeyboardCommand
    {
        public string Trigger => "command";
        
        public Task<IResult> Execute(Message msg, BotUser user)
        {
            var kb = new KeyboardBuilder(true);
            kb.AddButton("Расписание", "scheduleKeyboard", KeyboardButtonColor.Primary, "123");
            kb.AddButton("Экзамены", "exams", KeyboardButtonColor.Primary, "123");
            kb.AddLine();
            kb.AddButton("Погода на текущий момент", "weatherNow", KeyboardButtonColor.Primary, "123");
            kb.AddButton("Погода на день", "weatherDailyKeyboard", KeyboardButtonColor.Primary, "123");
            kb.AddLine();
            kb.AddButton("Напоминания", "reminds", KeyboardButtonColor.Default, "123");
            kb.AddButton("Справка", "help", KeyboardButtonColor.Default, "123");
            kb.AddButton("Рассылка", "mailingKeyboard", KeyboardButtonColor.Default, "123");

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Здравствуйте",
                Keyboard = kb.Build()
            });
        }
    }
}