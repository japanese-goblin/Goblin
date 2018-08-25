using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Goblin.Helpers;
using Goblin.Models;
using Newtonsoft.Json;

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
        public string Result { get; set; }

        private MainContext db = new MainContext();

        public void Execute(string param, int id = 0)
        {
            param = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(param);
            if (WeatherHelper.CheckCity(param))
            {
                var user = db.Users.First(x => x.Vk == id);
                user.City = param;
                db.SaveChanges();
                Result = $"Город успешно установлен на {param}";
            }
            else
            {
                Result = "Город не найден";
            }
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = "Введите название города";
                return false;
            }


            return true;
        }
    }

    //TODO: to JsonProperty
    class City
    {
        [JsonProperty("geoid")]
        public int GeoId { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
    }
}