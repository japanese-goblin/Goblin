using System.Threading.Tasks;
using Narfu;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class FindTeacher : ICommand
    {
        public string Name { get; } = "Найти *часть ФИО*";
        public string Decription { get; } = "Найти преподавателя по части его ФИО";
        public string Usage { get; } = "Найти деменков";
        public string[] Allias { get; } = { "найти" };
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var canExecute = CanExecute(msg);
            if(!canExecute.Success)
            {
                return new CommandResponse
                {
                    Text = canExecute.Text
                };
            }

            return new CommandResponse
            {
                Text = TeachersSchedule.FindByName(msg.GetParamsAsArray()[0])
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            var param = msg.GetParamsAsArray()[0];
            if(param.Length < 4)
            {
                return (false, "Ошибка. Введите больше 4х символов в ФИО");
            }

            var teachers = TeachersSchedule.FindByName(param);
            var found = !string.IsNullOrEmpty(teachers);
            if(!found)
            {
                var text = "Ошибка!\n" +
                           "Данного преподавателя нет в списке\n" +
                           "Если Вы уверены, что всё правильно, напишите мне для добавления препода в список (https://vk.com/id***REMOVED***)";

                return (false, text);
            }

            return (true, "");
        }
    }
}
