using System;
using System.Collections.Generic;
using System.Linq;
using Goblin.Models;

namespace Goblin.Bot.Commands
{
    public class AddRemindCommand : ICommand
    {
        public string Name { get; } = "Напомни *день*.*месяц*.*год* *час* *текст*";
        public string Decription { get; } = "Напоминает в указанное время о каком-то очень ВАЖНОМ тексте. День и месяц обязательно должны содержать 2 цифры, а год - 4. В указанное время бот напишет в личку сообщение с заданным текстом.";
        public string Usage { get; } = "Напомни 01.02.2018 15 зачет";
        public List<string> Allias { get; } = new List<string>() {"напомни"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var all = param.Split(' ');
            if (all.Length != 3)
            {
                Result = "Ошибочка";
                return;
            }

            if (!Utils.DevelopersID.Contains(id) && Utils.DB.Reminds.Count(x => x.VkID == id) > 7)
            {
                Result = "Превышен лимит (8) напоминалок";
                return;
            }
            var time = DateTime.Parse($"{all[0]} {all[1]}:00");
            var addedTime = time.AddHours(-3);
            Utils.DB.Reminds.Add(new Remind
            {
                Text = all[2], Date = addedTime, VkID = id
            });
            Utils.DB.SaveChanges();
            Result = $"Хорошо, {all[0]} в {all[1]}:00 напомню следующее:\n{all[2]}!";
        }
    }
}