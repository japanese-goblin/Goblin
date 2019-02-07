using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
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
        public Category Category => Category.Common;
        public bool IsAdmin => false;

        private readonly MainContext _db;
        public SetCity(MainContext db)
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

            var param = msg.GetParams(); // TODO первая буква была в верхнем регистре
            var text = "";
            if(await WeatherInfo.CheckCity(param))
            {
                var user = await _db.Users.FirstAsync(x => x.Vk == msg.FromId);
                user.City = param;
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
