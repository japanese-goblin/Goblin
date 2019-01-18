using System.Threading.Tasks;
using Narfu;
using Vk.Models.Keyboard;
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

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            Message = await TeachersSchedule.GetScheduleToSend(int.Parse(msg.GetParams()));
        }

        public bool CanExecute(Message msg)
        {
            if (int.TryParse(msg.GetParams(), out var res))
            {
                var find = TeachersSchedule.FindById(res);
                if (!find)
                {
                    Message = "Преподаватель с данным ID отсутствует";
                }

                return find;
            }

            Message = "Введите номер преподавателя!\n" +
                      "Получить его можно через команду 'Найти'\n";
            return false;
        }
    }
}