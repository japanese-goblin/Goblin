using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using OpenWeatherMap;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Weather : ICommand
    {
        public string Name { get; } = "Погода *название города*";
        public string Decription { get; } = "Возвращает погоду на текущее время";
        public string Usage { get; } = "Погода Москва";
        public string[] Allias { get; } = {"погода"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var param = msg.GetParams();
            var user = DbHelper.Db.Users.FirstOrDefault(x => x.Vk == msg.FromId);
            if (string.IsNullOrEmpty(param) && !string.IsNullOrEmpty(user?.City))
            {
                Message = await WeatherInfo.GetWeather(user.City);
                return;
            }

            if (await WeatherInfo.CheckCity(param))
            {
                Message = await WeatherInfo.GetWeather(param);
            }
            else
            {
                Message = "Город не найден (или ошибочка со стороны бота?)";
            }
        }

        public bool CanExecute(Message msg)
        {
            var user = DbHelper.Db.Users.FirstOrDefault(x => x.Vk == msg.FromId);
            if (string.IsNullOrEmpty(msg.GetParams()) && string.IsNullOrEmpty(user?.City))
            {
                Message = "Либо укажи город в параметре команды, либо установи его командой 'город'";
                return false;
            }

            return true;
        }
    }
}