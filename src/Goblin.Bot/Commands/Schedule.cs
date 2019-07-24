using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Domain.Entities;
using Narfu;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Schedule : ICommand
    {
        public string Name { get; } = "Раписание *день*.*месяц* ИЛИ Расписание завтра";

        public string Decription { get; } =
            "Возвращает расписание на указанную дату. Если дата не указана, расписание берется на текущую дату";

        public string Usage { get; } = "Расписание 21.12";
        public string[] Allias { get; } = { "расписание" };
        public CommandCategory Category { get; } = CommandCategory.Safu;
        public bool IsAdmin { get; } = false;

        private readonly NarfuService _service;

        public Schedule(NarfuService service)
        {
            _service = service;
        }

        public async Task<CommandResponse> Execute(Message msg, BotUser user)
        {
            var canExecute = CanExecute(msg, user);
            if(!canExecute.Success)
            {
                return new CommandResponse
                {
                    Text = canExecute.Text
                };
            }

            var param = msg.GetParams();
            DateTime time;
            if(string.IsNullOrEmpty(param))
            {
                time = DateTime.Now;
            }
            else if(param.Trim().ToLower() == "завтра")
            {
                time = DateTime.Now.AddDays(1);
            }
            else
            {
                var dayAndMonth = param.Split('.').Select(int.Parse).ToArray(); // [Day, Month]
                time = new DateTime(DateTime.Now.Year, dayAndMonth[1], dayAndMonth[0]);
            }

            return new CommandResponse
            {
                Text = await _service.Students.GetScheduleAsStringAtDate(time, user.Group)
            };
        }

        public (bool Success, string Text) CanExecute(Message msg, BotUser user)
        {
            var param = msg.GetParams();
            if(user.Group == 0)
            {
                return (false,
                        "Ошибка. Для просмотра расписания необходимо установить группу командой 'устгр *номер группы*' (без кавычек и звездочек - устгр 353535, например)");
            }

            if(param == "" || param.ToLower() == "завтра")
            {
                return (true, "");
            }

            var date = param.Split('.');
            if(date.Length != 2)
            {
                return (false, $"Ошибка. Указана неправильная дата. Пример использования команды: {Usage}");
            }

            var isGoodDate = DateTime.TryParseExact($"{date[0]}.{date[1]}",
                                                    new[] { "d.M", "d.MM", "dd.M", "dd.MM" },
                                                    null, DateTimeStyles.None, out _);

            if(!isGoodDate)
            {
                return (false, $"Ошибка. Указана неправильная дата Пример использования команды: {Usage}");
            }

            return (true, "");
        }
    }
}