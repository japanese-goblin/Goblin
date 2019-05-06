using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Persistence;
using Microsoft.EntityFrameworkCore;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SetMailing : ICommand
    {
        public string Name { get; } = "Подписка *расписание/погода*";
        public string Decription { get; } = "Подписка на рассылку расписания/погоды (ЧТО-ТО ОДНО ЗА РАЗ)";
        public string Usage { get; } = "Подписка расписание";
        public string[] Allias { get; } = { "подписка" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        private readonly ApplicationDbContext _db;

        public SetMailing(ApplicationDbContext db)
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

            var param = msg.GetParamsAsArray()[0].ToLower();
            var text = "";
            if(param == "погода")
            {
                var user = await _db.BotUsers.FirstAsync(x => x.Vk == msg.FromId);

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
                var user = await _db.BotUsers.FirstAsync(x => x.Vk == msg.FromId);
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

        public (bool Success, string Text) CanExecute(Message msg)
        {
            if(string.IsNullOrEmpty(msg.GetParams()))
            {
                return (false, "А на что подписаться? Укажи 'погода' либо 'расписание'");
            }

            return (true, "");
        }
    }
}