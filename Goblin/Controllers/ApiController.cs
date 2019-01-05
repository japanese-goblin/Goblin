using Goblin.Vk.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goblin.Controllers
{
    public class ApiController : Controller
    {
        public async Task<string> Handler([FromBody] Response resp)
        {
            return await Bot.Handler.Handle(resp);
        }

        //public async Task SendWeather()
        //{
        //    await Task.Factory.StartNew(async () =>
        //    {
        //        var grouped = DbHelper.GetWeatherUsers().GroupBy(x => x.City);
        //        foreach (var group in grouped)
        //        {
        //            var ids = group.Select(x => x.Vk).ToArray();
        //            await VkMethods.SendMessage(ids, await WeatherInfo.GetWeather(group.Key));
        //            await Task.Delay(700); //TODO - 3 запроса в секунду
        //        }
        //    });
        //}

        //public async Task SendSchedule()
        //{
        //    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return;
        //    await Task.Factory.StartNew(async () =>
        //    {
        //        var grouped = DbHelper.GetScheduleUsers().GroupBy(x => x.Group);
        //        foreach (var group in grouped)
        //        {
        //            var ids = group.Select(x => x.Vk).ToArray();
        //            var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Today, group.Key);
        //            await VkMethods.SendMessage(ids, schedule);
        //            await Task.Delay(500); //TODO - 3 запроса в секунду
        //        }
        //    });
        //}

        //public async Task SendToConv(int id, int group = 0, string city = "")
        //{
        //    if (!StudentsSchedule.IsCorrectGroup(group)) return;

        //    id = 2000000000 + id;

        //    var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Now, group);
        //    await VkMethods.SendMessage(id, schedule);

        //    if (!string.IsNullOrEmpty(city) && await WeatherInfo.CheckCity(city))
        //    {
        //        var weather = await WeatherInfo.GetWeather(city);
        //        await VkMethods.SendMessage(id, weather);
        //    }
        //}
    }
}