using System;
using System.Globalization;
using System.Threading.Tasks;
using Goblin.Data.Enums;
using Goblin.Data.Models;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class AddRemind : ICommand
    {
        public string Name { get; } = "Напомни *день*.*месяц*.*год* *часы*:*минуты* *текст*";

        public string Decription { get; } =
            "Напоминает в указанное время о каком-то очень ВАЖНОМ тексте. " +
            "День и месяц обязательно должны содержать 2 цифры, а год - 4. " +
            "В указанное время бот напишет в личку сообщение с заданным текстом.";

        public string Usage { get; } = "Напомни 21.12.2018 15:35 зачет";
        public string[] Allias { get; } = { "напомни" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        private readonly MainContext _db;

        public AddRemind(MainContext db)
        {
            _db = db;
        }

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

            var param = msg.GetParams();
            var all = param.Split(' ', 3);
            if(all[0].ToLower() == "завтра") //TODO: поменять
            {
                var d = DateTime.Now.AddDays(1);
                all[0] = $"{d.Day}.{d.Month}.{d.Year}";
            }

            if(all[0].ToLower() == "сегодня") //TODO: поменять
            {
                var d = DateTime.Now;
                all[0] = $"{d.Day}.{d.Month}.{d.Year}";
            }

            var time = ParseTime(all[0], all[1]);
            await _db.Reminds.AddAsync(new Remind
            {
                Text = all[2],
                Date = time.Result,
                VkId = msg.FromId
            });
            await _db.SaveChangesAsync();

            return new CommandResponse
            {
                Text = $"Хорошо, {time.Result:dd.MM.yyyy (dddd)} в {time.Result:HH:mm} напомню следующее:\n{all[2]}"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            var param = msg.GetParams();
            var all = param.Split(' ', 3);
            if(all.Length != 3)
            {
                return (false, $"Ошибка. Укажите дату, время и текст напоминания ({Usage})");
            }

            if(all[0].ToLower() == "завтра" || all[0].ToLower() == "сегодня")
            {
                return (true, "");
            }

            //TODO: все равно никто не пользуется ех
            //if (!VkHelper.DevelopersID.Contains(id) && _db.Reminds.Count(x => x.VkID == id) > 7)
            //{
            //    Message = "Превышен лимит (8) напоминалок";
            //    return false;
            //}

            /*
             * M, MM - месяцы
             * d, dd - дни
             * H, HH - часы
             * m, mm - минуты
             * yyyy - год
             */

            var time = ParseTime(all[0], all[1]);
            if(!time.IsGood)
            {
                return (false, $"Ошибка. Введена дата неверного формата. Пример корректной даты: {Usage}");
            }

            if(time.Result < DateTime.Now)
            {
                return (false, "Ошибка. Дата меньше текущей.");
            }

            return (true, "");
        }

        private (bool IsGood, DateTime Result) ParseTime(string date, string time)
        {
            var isGood = DateTime.TryParseExact($"{date} {time}",
                                                new[]
                                                {
                                                    "dd.MM.yyyy HH:mm", "d.MM.yyyy HH:mm",
                                                    "dd.M.yyyy HH:mm", "d.M.yyyy HH:mm",
                                                    "dd.MM.yyyy H:mm", "d.MM.yyyy H:mm",
                                                    "dd.M.yyyy H:mm", "d.M.yyyy H:mm",
                                                    "dd.MM.yyyy HH:m", "d.MM.yyyy HH:m",
                                                    "dd.M.yyyy HH:m", "d.M.yyyy HH:m",
                                                    "dd.MM.yyyy H:m", "d.MM.yyyy H:m",
                                                    "dd.M.yyyy H:m", "d.M.yyyy H:m"
                                                },
                                                null, DateTimeStyles.None, out var res);

            return (isGood, res);
        }
    }
}