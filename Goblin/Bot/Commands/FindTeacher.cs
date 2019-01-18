using System.Threading.Tasks;
using Narfu;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class FindTeacher : ICommand
    {
        public string Name { get; } = "Найти *часть ФИО*";
        public string Decription { get; } = "Найти преподавателя по части его ФИО";
        public string Usage { get; } = "Найти деменков";
        public string[] Allias { get; } = {"найти"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            Message = TeachersSchedule.FindByName(msg.GetParamsAsArray()[0]);
        }

        public bool CanExecute(Message msg)
        {
            if (msg.Text.Length < 4)
            {
                Message = "Введите больше символов в ФИО";
                return false;
            }
            var x = TeachersSchedule.FindByName(msg.GetParamsAsArray()[0]);
            var find = string.IsNullOrEmpty(x);
            if (find)
            {
                Message = "Ошибка!\n" +
                          "Данного преподавателя нет в списке\n" +
                          "Если Вы уверены, что всё правильно, напишите мне для добавления препода в список (https://vk.com/id***REMOVED***)";
            }

            return !find;
        }
    }
}