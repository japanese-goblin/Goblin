using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
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
        public Weather(MainContext db)
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
            var user = _db.Users.FirstOrDefault(x => x.Vk == msg.FromId);
            if(string.IsNullOrEmpty(param) && !string.IsNullOrEmpty(user?.City))
            {
                return new CommandResponse
                {
                    Text = await WeatherInfo.GetWeather(user.City)
                };
            }

            var text = "";
            if(await WeatherInfo.CheckCity(param))
            {
                text = await WeatherInfo.GetWeather(param);
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
