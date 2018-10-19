using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using Goblin.Models.Keyboard;

namespace Goblin.Bot.Commands
{
    public class AddRemindCommand : ICommand
    {
        public string Name { get; } = "Напомни *день*.*месяц*.*год* *час*:*минута* *текст*";
        public string Decription { get; } = "Напоминает в указанное время о каком-то очень ВАЖНОМ тексте. День и месяц обязательно должны содержать 2 цифры, а год - 4. В указанное время бот напишет в личку сообщение с заданным текстом.";
        public string Usage { get; } = "Напомни 21.12.2018 15:35 зачет";
        public List<string> Allias { get; } = new List<string> {"напомни"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;
        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            var all = param.Split(' ', 3);
            if (all[0].ToLower() == "завтра") //TODO: поменять
            {
                var d = DateTime.Now.AddDays(1);
                all[0] = $"{d.Day}.{d.Month}.{d.Year}";
            }

            var time = ParseTime(all[0], all[1]);
            await DbHelper.Db.Reminds.AddAsync(new Remind
            {
                Text = all[2],
                Date = time.Result,
                VkID = id
            });
            await DbHelper.Db.SaveChangesAsync();
            Message = $"Хорошо, {time.Result:dd.MM.yyyy} в {time.Result:HH:mm} напомню следующее:\n{all[2]}";
        }

        public bool CanExecute(string param, int id = 0)
        {
            var all = param.Split(' ', 3);
            if (all.Length != 3)
            {
                Message = "Ошибочка";
                return false;
            }

            if (all[0].ToLower() == "завтра")
            {
                var d = DateTime.Now.AddDays(1);
                all[0] = $"{d.Day}.{d.Month}.{d.Year}";
            }

            //TODO: все равно никто не пользуется ех
            //if (!VkHelper.DevelopersID.Contains(id) && DbHelper.Db.Reminds.Count(x => x.VkID == id) > 7)
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
            if (!time.IsGood)
            {
                Message = $"Введена неправильная дата. Пример использования команды: {Usage}";
                return false;
            }

            if (time.Result < DateTime.Now)
            {
                Message = "Дата меньше текущей.";
                return false;
            }

            return true;
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