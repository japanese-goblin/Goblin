using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
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
        public CommandCategory Category { get; } = CommandCategory.Safu;
        public bool IsAdmin { get; } = false;

        public Task<CommandResponse> Execute(Message msg)
        {
            var canExecute = CanExecute(msg);
            if(!canExecute.Success)
            {
                return Task.Run(() => new CommandResponse
                {
                    Text = canExecute.Text
                });
            }

            var text = TeachersSchedule.FindByName(msg.GetParamsAsArray()[0]);
            if(text.Length >= 4095)
            {
                text =
                    "Ошибка. Найдено слишком много преподавателей.\n" +
                    "Пожалуйста, укажите более точные данные (например, введите фамилию и имя преподавателя).";
            }

            return Task.Run(() => new CommandResponse
            {
                Text = text
            });
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
                           "Данного преподавателя нет в списке";

                return (false, text);
            }

            return (true, "");
        }
    }
}