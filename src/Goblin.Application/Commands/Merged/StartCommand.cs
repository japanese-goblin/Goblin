using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Commands.Merged
{
    public class StartCommand : IKeyboardCommand, ITextCommand
    {
        public string Trigger => "command";

        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "старт" };

        public Task<IResult> Execute(Message msg, BotUser user)
        {
            var kb = new KeyboardBuilder(true);
            kb.AddButton("Расписание", "", KeyboardButtonColor.Primary, "scheduleKeyboard");
            kb.AddButton("Экзамены", "", KeyboardButtonColor.Primary, "exams");
            kb.AddLine();
            kb.AddButton("Погода на текущий момент", "", KeyboardButtonColor.Primary, "weatherNow");
            kb.AddButton("Погода на день", "", KeyboardButtonColor.Primary, "weatherDailyKeyboard");
            kb.AddLine();
            kb.AddButton("Рассылка", "", KeyboardButtonColor.Primary, "mailingKeyboard");
            kb.AddButton("Напоминания", "", KeyboardButtonColor.Default, "reminds");
            kb.AddButton("Справка", "", KeyboardButtonColor.Primary, "help");

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "Выберите действие:",
                Keyboard = kb.Build()
            });
        }
    }
}