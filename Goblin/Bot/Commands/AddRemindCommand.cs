using System;
using System.Collections.Generic;
using System.Linq;
using Goblin.Models;

namespace Goblin.Bot.Commands
{
    public class AddRemindCommand : ICommand
    {
        public string Name { get; } = "Напомни";
        public string Decription { get; } = "напоминалка";
        public string Usage { get; } = "Напомни 01.02.2018 15:15 зачет";
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
                Result = "Превышен лимит напоминалок";
                return;
            }
            var time = DateTime.Parse($"{all[0]} {all[1]}");
            var addedTime = time.AddHours(3);
            Utils.DB.Reminds.Add(new Remind
            {
                Text = all[2], Timestamp = new DateTimeOffset(addedTime).ToUnixTimeSeconds(), VkID = id
            });
            Utils.DB.SaveChanges();
            Result = $"Хорошо, {all[1]} в {all[2]} напомню следующее:\n{all[3]}!";
        }
    }
}