using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Persistence;
using Microsoft.EntityFrameworkCore;
using OpenWeatherMap;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SetCity : ICommand
    {
        public string Name => "Город *название города*";
        public string Decription => "Установка города для получения рассылки погоды";
        public string Usage => "Город Москва";
        public string[] Allias { get; } = { "город" };
        public CommandCategory Category => CommandCategory.Common;
        public bool IsAdmin => false;

        private readonly ApplicationDbContext _db;
        private readonly WeatherInfo _weather;

        public SetCity(ApplicationDbContext db, WeatherInfo weather)
        {
            _db = db;
            _weather = weather;
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
            var text = "";
            if(await _weather.CheckCity(param))
            {
                var user = await _db.BotUsers.FirstAsync(x => x.Vk == msg.FromId);
                user.City = char.ToUpper(param[0]) + param.Substring(1).ToLower(); //чтобы первая буква была с боьшой буквы (потом для группировки пригодится)
                await _db.SaveChangesAsync();
                text = $"Город успешно установлен на {param}";
            }
            else
            {
                text = $"Ошибка. Город '{msg.GetParams()}' не найден";
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
                return (false, "Ошибка. Введите название города");
            }

            return (true, "");
        }
    }
}