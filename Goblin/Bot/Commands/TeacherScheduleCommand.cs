using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Vk.Models;

namespace Goblin.Bot.Commands
{
    public class TeacherScheduleCommand : ICommand
    {
        public string Name { get; } = "Препод *номер*";
        public string Decription { get; } = "Поиск препода по его номеру ИЛИ получение расписания у препода";

        public string Usage { get; } = "Препод 22331";
        public List<string> Allias { get; } = new List<string>() {"препод"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            Message = await TeacherScheduleHelper.GetScheduleToSend(int.Parse(param));
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (int.TryParse(param, out var res))
            {
                var find = TeacherScheduleHelper.FindById(res);
                if (!find)
                    Message = "Преподаватель с данным ID отсутствует";
                return find;
            }
            else
            {
                Message = "Введите номер преподавателя!\n" +
                          "Получить его можно через команду 'Найти'\n";
                return false;
            }
        }
    }
}