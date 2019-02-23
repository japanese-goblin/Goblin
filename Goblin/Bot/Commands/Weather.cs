using System.Linq;
using System.Threading.Tasks;
using Goblin.Data.Enums;
using Goblin.Data.Models;
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
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        private readonly MainContext _db;
        private readonly WeatherInfo _weather;

        public Weather(MainContext db, WeatherInfo weather)
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
            var user = _db.Users.FirstOrDefault(x => x.Vk == msg.FromId);
            if(string.IsNullOrEmpty(param) && !string.IsNullOrEmpty(user?.City))
            {
                return new CommandResponse
                {
                    Text = await _weather.GetWeather(user.City)
                };
            }

            var text = "";
            if(await _weather.CheckCity(param))
            {
                text = await _weather.GetWeather(param);
            }
            else
            {
                text = $"Ошибка. Город '{param}' не найден (или ошибочка со стороны бота?)";
            }

            return new CommandResponse
            {
                Text = text
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            var user = _db.Users.FirstOrDefault(x => x.Vk == msg.FromId);
            if(string.IsNullOrEmpty(msg.GetParams()) && string.IsNullOrEmpty(user?.City))
            {
                return (false, "Ошибка. Либо укажи город в команде через пробел, либо установи его командой 'город'");
            }

            return (true, "");
        }
    }
}