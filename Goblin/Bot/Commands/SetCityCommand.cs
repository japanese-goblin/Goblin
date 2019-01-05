using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Goblin.Helpers;
using Vk.Models.Keyboard;
using Microsoft.EntityFrameworkCore;
using OpenWeatherMap;

namespace Goblin.Bot.Commands
{
    public class SetCityCommand : ICommand
    {
        public string Name => "Город *название города*";
        public string Decription => "Установка города для получения рассылки погоди";
        public string Usage => "Город Москва";
        public List<string> Allias => new List<string> {"город"};
        public Category Category => Category.Common;
        public bool IsAdmin => false;
        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            param = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(param);
            if (await WeatherInfo.CheckCity(param))
            {
                var user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == id);
                user.City = param;
                await DbHelper.Db.SaveChangesAsync();
                Message = $"Город успешно установлен на {param}";
            }
            else
            {
                Message = "Город не найден";
            }
        }

        public bool CanExecute(string param, long id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Message = "Введите название города";
                return false;
            }


            return true;
        }
    }
}