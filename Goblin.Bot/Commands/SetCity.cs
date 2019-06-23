using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Domain.Entities;
using Goblin.Persistence;
using OpenWeatherMap;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SetCity : ICommand
    {
        public string Name { get; } = "Город *название города*";
        public string Description { get; } = "Установка города для получения рассылки погоды";
        public string Usage { get; } = "Город Москва";
        public string[] Aliases { get; } = { "город" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        private readonly BotDbContext _db;
        private readonly WeatherService _weather;

        public SetCity(BotDbContext db, WeatherService weather)
        {
            _db = db;
            _weather = weather;
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
            var text = "";
            if(await _weather.CheckCity(param))
            {
                user.City = char.ToUpper(param[0]) +
                            param.Substring(1).ToLower(); //чтобы первая буква была с боьшой буквы (потом для группировки пригодится)
                await _db.SaveChangesAsync();
                text = $"Город успешно установлен на {user.City}";
            }
            else
            {
                text = $"Ошибка. Город '{param}' не найден";
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
                return (false, "Ошибка. Введите название города");
            }

            return (true, "");
        }
    }
}