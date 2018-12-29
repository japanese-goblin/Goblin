using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models.Keyboard;

namespace Goblin.Bot.Commands
{
    public class FindTeacherCommand : ICommand
    {
        public string Name { get; } = "Найти *часть ФИО*";
        public string Decription { get; } = "Найти преподавателя по части его ФИО";
        public string Usage { get; } = "Найти деменков";
        public List<string> Allias { get; } = new List<string>() {"найти"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            Message = TeacherScheduleHelper.FindByName(param);
        }

        public bool CanExecute(string param, int id = 0)
        {
            var x = TeacherScheduleHelper.FindByName(param);
            var find = string.IsNullOrEmpty(x);
            if (find)
                Message = "Ошибка!\n" +
                          "Данного преподавателя нет в списке\n" +
                          "Если Вы уверены, что всё правильно, напишите мне для добавления препода в список (https://vk.com/id***REMOVED***)";
            return !find;
        }
    }
}