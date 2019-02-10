using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using Narfu;
using OpenWeatherMap;
using Vk;

namespace Goblin
{
    public class ScheduledTasks
    {
        private readonly MainContext _db;
        private readonly VkApi _api;
        private readonly WeatherInfo _weather;

        public ScheduledTasks(MainContext db, VkApi api, WeatherInfo weather)
        {
            _db = db;
            _api = api;
            _weather = weather;
        }

        public async Task SendRemind()
        {
            var reminds =
                _db.Reminds
                   .AsNoTracking()
                   .Where(x => $"{x.Date:dd.MM.yyyy HH:mm}" == $"{DateTime.Now:dd.MM.yyyy HH:mm}");

            if(!reminds.Any()) return;

            foreach(var remind in reminds)
            {
                await _api.Messages.Send(remind.VkID, $"Напоминаю:\n {remind.Text}");
                _db.Reminds.Remove(remind);
            }

            await _db.SaveChangesAsync();
        }

        public async Task SendSchedule()
        {
            //TODO: это теперь задается в кроне
            //if(DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return;

            await Task.Factory.StartNew(async () =>
            {
                var grouped = _db.GetScheduleUsers().GroupBy(x => x.Group);
                foreach(var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToArray();
                    var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Today, group.Key);
                    await _api.Messages.Send(ids, schedule);
                    await Task.Delay(100); //TODO: 20 запросов в секунду
                }
            });
        }

        public async Task SendWeather()
        {
            await Task.Factory.StartNew(async () =>
            {
                var grouped = _db.GetWeatherUsers().GroupBy(x => x.City);
                foreach(var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToArray();
                    await _api.Messages.Send(ids, await _weather.GetWeather(group.Key));
                    await Task.Delay(100); //TODO: 20 запроса в секунду
                }
            });
        }

        public async Task SendToConv(int id, int group = 0, string city = "")
        {
            //TODO
            if(!StudentsSchedule.IsCorrectGroup(group))
            {
                return;
            }

            id = 2000000000 + id;

            var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Now, group);
            await _api.Messages.Send(id, schedule);

            if(!string.IsNullOrEmpty(city) && await _weather.CheckCity(city))
            {
                var weather = await _weather.GetWeather(city);
                await _api.Messages.Send(id, weather);
            }
        }
    }
}