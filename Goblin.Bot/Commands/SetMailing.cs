using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Domain.Entities;
using Goblin.Persistence;
using Microsoft.EntityFrameworkCore;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SetMailing : ICommand
    {
        public string Name { get; } = "Подписка *расписание ИЛИ погода*";
        public string Decription { get; } = "Подписка на рассылку расписания или погоды";
        public string Usage { get; } = "Подписка расписание";
        public string[] Allias { get; } = { "подписка" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        private readonly ApplicationDbContext _db;

        public SetMailing(ApplicationDbContext db)
        {
            _db = db;
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

            var param = msg.GetParamsAsArray()[0].ToLower();
            var text = "";
            if(param == "погода")
            {
                if(!string.IsNullOrEmpty(user.City))
                {
                    user.Weather = true;
                    text = "Ты успешно подписался на рассылку погоды!\n" +
                           $"Город для получения погоды - {user.City}";
                }
                else
                {
                    text = "Для того, чтобы подписаться на рассылку погоды, необходимо установить город для рассылки" +
                           "командой 'город' (смотри команды для более полной информации)";
                }
            }
            else if(param == "расписание")
            {
                if(user.Group != 0)
                {
                    user.Schedule = true;
                    text = "Ты успешно подписался на рассылку расписания!\n" +
                           $"Группа для получения расписания - {user.Group}";
                }
                else
                {
                    text = "Для того, чтобы подписаться на рассылку расписания, необходимо установить группу" +
                           "командой 'устгр' (смотри команды для более полной информации)";
                }
            }
            else
            {
                text = $"Ошибка. Можно подписаться на рассылку погоды или расписания (выбрано - {param})";
            }

            if(_db.ChangeTracker.HasChanges())
            {
                await _db.SaveChangesAsync();
            }

            return new CommandResponse
            {
                Text = text
            };
        }

        public (bool Success, string Text) CanExecute(Message msg, BotUser user)
        {
            if(string.IsNullOrEmpty(msg.GetParams()))
            {
                return (false, "А на что подписаться? Укажи 'погода' либо 'расписание'");
            }

            return (true, "");
        }
    }
}