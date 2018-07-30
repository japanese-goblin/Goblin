using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

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

        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var cityid = 0;
            var user = Utils.DB.Users.First(x => x.Vk == id);
            if (string.IsNullOrEmpty(param) && user.CityNumber != 0)
            {
                Result = GetWeather(user.City, user.CityNumber);
                return;
            }

            if (CheckCity(param, ref cityid))
            {
                Result = GetWeather(param, cityid);
            }
            else
            {
                Result = "Город не найден";
            }
        }

        public bool CanExecute(string param, int id = 0)
        {
            var user = Utils.DB.Users.First(x => x.Vk == id);
            if (string.IsNullOrEmpty(param) && user.CityNumber == 0)
            {
                Result = "Либо укажи город в параметре команды, либо установи его командой 'город'";
                return false;
            }

            return true;
        }

        //TODO: вынести в отдельный класс
        private string GetWeather(string city, int cityid)
        {
            var weather = "";
            var info = GetCityInfo(cityid);
            var all = JsonConvert.DeserializeObject<dynamic>(info);
            var fact = JsonConvert.DeserializeObject<dynamic>(all["fact"].ToString());
            var conditions = JsonConvert.DeserializeObject<dynamic>(all["l10n"].ToString());

            //TODO: fix time?
            weather = $"Погода в городе {city} на данный момент:\n" +
                      $"Температура: {fact["temp"]}°С. Ощущается как {fact["feels_like"]}°С.\n" +
                      $"Описание погоды: {conditions[fact["condition"].ToString()]}\n" +
                      $"Влажность: {fact["humidity"]}%\n" +
                      $"Ветер: {fact["wind_speed"]} м/с\n" +
                      $"Давление: {fact["pressure_mm"]} мм рт.ст.";

            return weather;
        }

        private dynamic GetCityInfo(int cityid)
        {
            var uuid = "8211637137c4408898aceb1097921872";
            var deviceid = "315f0e802b0b49eb8404ea8056abeaaf";
            var time = DateTimeOffset.Now.ToUnixTimeSeconds();
            var token = Utils.CreateMD5($"eternalsun{time}");
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "yandex-weather-android/4.2.1");
                client.Headers.Add("Accept-Charset", "utf-8");
                client.Headers.Add("X-Yandex-Weather-Client", "YandexWeatherAndroid/4.2.1");
                client.Headers.Add("X-Yandex-Weather-Device",
                    $"os=null;os_version=21;manufacturer=chromium;model=App Runtime for Chrome Dev;device_id={deviceid};uuid={uuid};");
                client.Headers.Add("X-Yandex-Weather-Token", token);
                client.Headers.Add("X-Yandex-Weather-Timestamp", time.ToString());
                client.Headers.Add("X-Yandex-Weather-UUID", uuid);
                client.Headers.Add("X-Yandex-Weather-Device-ID", deviceid);
                //client.Headers.Add("Accept-Encoding", "gzip, deflate");
                //client.Headers.Add("Host", "api.weather.yandex.ru");
                //client.Headers.Add("Connection", "Keep-Alive");

                //https://api.weather.yandex.ru/v1/forecast?geoid=213&l10n=true&extra=true
                var url = $"https://api.weather.yandex.ru/v1/forecast?geoid={cityid}&hours=false&l10n=true";
                var a = client.DownloadString(url);
                return a;
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
                client.Headers.Add("X-Yandex-Weather-Device",
                    $"os=null;os_version=21;manufacturer=chromium;model=App Runtime for Chrome Dev;device_id={deviceid};uuid={uuid};");
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
                cityid = selected.GeoId;
                return true;
            }
        }
    }
}