using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Goblin.Bot.Commands
{
    public class SetCityCommand : ICommand
    {
        public string Name => "город";
        public string Decription => "Установка города для получения рассылки";
        public string Usage => "город Москва";
        public List<string> Allias => new List<string>() { "город" };
        public Category Category => Category.Common;
        public bool IsAdmin => false;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            int cityid = 0;
            param = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(param);
            if (CheckCity(param, ref cityid))
            {
                var user = Utils.DB.Users.First(x => x.Vk == id);
                user.CityNumber = cityid;
                user.City = param;
                Utils.DB.SaveChanges();
                Result = $"Город успешно установлен на {param}";
            }
            else
            {
                Result = "Город не найден";
            }
        }

        private bool CheckCity(string city, ref int cityid)
        {
            city = city.ToLower();
            var uuid = "8211637137c4408898aceb1097921872";
            var deviceid = "315f0e802b0b49eb8404ea8056abeaaf";
            var time = DateTimeOffset.Now.ToUnixTimeSeconds();
            var token = Utils.CreateMD5($"eternalsun{time}");
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "yandex-weather-android/4.2.1");
                client.Headers.Add("Accept-Charset", "utf-8");
                client.Headers.Add("X-Yandex-Weather-Client", "YandexWeatherAndroid/4.2.1");
                client.Headers.Add("X-Yandex-Weather-Device", $"os=null;os_version=21;manufacturer=chromium;model=App Runtime for Chrome Dev;device_id={deviceid};uuid={uuid};");
                client.Headers.Add("X-Yandex-Weather-Token", token);
                client.Headers.Add("X-Yandex-Weather-Timestamp", time.ToString());
                client.Headers.Add("X-Yandex-Weather-UUID", uuid);
                client.Headers.Add("X-Yandex-Weather-Device-ID", deviceid);
                //client.Headers.Add("Accept-Encoding", "gzip, deflate");
                //client.Headers.Add("Host", "api.weather.yandex.ru");
                //client.Headers.Add("Connection", "Keep-Alive");

                var a = client.DownloadString("https://api.weather.yandex.ru/v1/locations?lang=ru_RU");
                var citys = JsonConvert.DeserializeObject<List<City>>(a);
                var selected = citys.FirstOrDefault(x => x.name.ToLower() == city);
                if (selected == null) return false;
                cityid = selected.geoid;
                return true;
            }
        }
    }

    class City
    {
        public int geoid { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
    }
}
