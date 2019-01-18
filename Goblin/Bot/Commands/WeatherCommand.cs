﻿using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using OpenWeatherMap;
using Vk.Models.Keyboard;

namespace Goblin.Bot.Commands
{
    public class WeatherCommand : ICommand
    {
        public string Name { get; } = "Погода *название города*";
        public string Decription { get; } = "Возвращает погоду на текущее время";
        public string Usage { get; } = "Погода Москва";
        public string[] Allias { get; } = {"погода"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var user = DbHelper.Db.Users.FirstOrDefault(x => x.Vk == id);
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

        public bool CanExecute(string param, long id = 0)
        {
            var user = DbHelper.Db.Users.FirstOrDefault(x => x.Vk == id);
            if (string.IsNullOrEmpty(param) && string.IsNullOrEmpty(user?.City))
            {
                Message = "Либо укажи город в параметре команды, либо установи его командой 'город'";
                return false;
            }

            return true;
        }
    }
}