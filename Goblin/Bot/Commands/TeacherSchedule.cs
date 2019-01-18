using System.Threading.Tasks;
using Narfu;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class TeacherSchedule : ICommand
    {
        public string Name { get; } = "Препод *номер*";
        public string Decription { get; } = "Поиск препода по его номеру ИЛИ получение расписания у препода";

        public string Usage { get; } = "Препод 22331";
        public string[] Allias { get; } = {"препод"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var canExecute = CanExecute(msg);
            if (!canExecute.Success)
            {
                return new CommandResponse
                {
                    Text = canExecute.Text
                };
            }

            return new CommandResponse
            {
                Text = await TeachersSchedule.GetScheduleToSend(int.Parse(msg.GetParams()))
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            if (int.TryParse(msg.GetParams(), out var res))
            {
                var isFound = TeachersSchedule.FindById(res);
                if (!isFound)
                {
                    return (false, "Ошибка. Преподаватель с таким номером нет в базе");
                }

                return (true, "");
            }

            return (false, "Введите номер преподавателя!\n" +
                           "Получить его можно через команду 'Найти'\n");
        }
    }
}