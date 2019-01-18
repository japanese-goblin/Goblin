using System.Globalization;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using OpenWeatherMap;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SetCity : ICommand
    {
        public string Name => "Город *название города*";
        public string Decription => "Установка города для получения рассылки погоды";
        public string Usage => "Город Москва";
        public string[] Allias { get; } = {"город"};
        public Category Category => Category.Common;
        public bool IsAdmin => false;
        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var param = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(msg.GetParams());
            if (await WeatherInfo.CheckCity(param))
            {
                var user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == msg.FromId);
                user.City = param;
                await DbHelper.Db.SaveChangesAsync();
                Message = $"Город успешно установлен на {param}";
            }
            else
            {
                Message = "Город не найден";
            }
        }

        public bool CanExecute(Message msg)
        {
            if (string.IsNullOrEmpty(msg.GetParams()))
            {
                Message = "Введите название города";
                return false;
            }

            return true;
        }
    }
}