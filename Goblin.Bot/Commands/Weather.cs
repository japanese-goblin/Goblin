using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Persistence;
using OpenWeatherMap;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Weather : ICommand
    {
        public string Name { get; } = "Погода *название города*";
        public string Decription { get; } = "Возвращает погоду на текущее время";
        public string Usage { get; } = "Погода Москва";
        public string[] Allias { get; } = { "погода" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        private readonly ApplicationDbContext _db;
        private readonly WeatherInfo _weather;

        public Weather(ApplicationDbContext db, WeatherInfo weather)
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

            string response;
            if(msg.GetParams().ToLower() == "завтра")
            {
                response = await ExecuteDaily(msg);
            }
            else
            {
                response = await ExecuteNow(msg);
            }

            return new CommandResponse
            {
                Text = response
            };
        }

        private async Task<string> ExecuteNow(Message msg)
        {
            var param = msg.GetParams();
            var user = _db.BotUsers.FirstOrDefault(x => x.Vk == msg.FromId);
            if(string.IsNullOrEmpty(param) && !string.IsNullOrEmpty(user?.City))
            {
                return await _weather.GetCurrentWeather(user.City);
            }

            var text = "";
            if(await _weather.CheckCity(param))
            {
                text = await _weather.GetCurrentWeather(param);
            }
            else
            {
                text = $"Ошибка. Город '{param}' не найден (или ошибочка со стороны бота?)";
            }

            return text;
        }

        private async Task<string> ExecuteDaily(Message msg)
        {
            var user = _db.BotUsers.FirstOrDefault(x => x.Vk == msg.FromId);
            if(!string.IsNullOrEmpty(user?.City))
            {
                return await _weather.GetDailyWeather(user.City, DateTime.Today.AddDays(1));
            }

            return "Ошибка. Для просмотра погоды на завтра укажи город через команду 'город *название*'";
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            var user = _db.BotUsers.FirstOrDefault(x => x.Vk == msg.FromId);
            if(string.IsNullOrEmpty(msg.GetParams()) && string.IsNullOrEmpty(user?.City))
            {
                return (false, "Ошибка. Либо укажи город в команде через пробел, либо установи его командой 'город'");
            }

            return (true, "");
        }
    }
}