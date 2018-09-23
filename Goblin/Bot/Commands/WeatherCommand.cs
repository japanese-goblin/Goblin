using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using VkNet.Model.Keyboard;

namespace Goblin.Bot.Commands
{
    public class WeatherCommand : ICommand
    {
        public string Name { get; } = "Погода *название города*";
        public string Decription { get; } = "Возвращает погоду на текущее время";
        public string Usage { get; } = "Погода Москва";
        public List<string> Allias { get; } = new List<string> {"погода"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public MessageKeyboard Keyboard { get; set; }

        private MainContext db = new MainContext();

        public async Task Execute(string param, int id = 0)
        {
            var user = db.Users.FirstOrDefault(x => x.Vk == id);
            if (string.IsNullOrEmpty(param) && !string.IsNullOrEmpty(user?.City))
            {
                Message = await WeatherHelper.GetWeather(user?.City);
                return;
            }

            if (await WeatherHelper.CheckCity(param))
            {
                Message = await WeatherHelper.GetWeather(param);
            }
            else
            {
                Message = "Город не найден (или ошибочка со стороны бота?)";
            }
        }

        public bool CanExecute(string param, int id = 0)
        {
            var user = db.Users.FirstOrDefault(x => x.Vk == id);
            if (string.IsNullOrEmpty(param) && string.IsNullOrEmpty(user?.City))
            {
                Message = "Либо укажи город в параметре команды, либо установи его командой 'город'";
                return false;
            }

            return true;
        }
    }
}