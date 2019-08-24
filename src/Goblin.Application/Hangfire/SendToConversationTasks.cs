using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using Hangfire;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class SendToConversationTasks
    {
        private readonly BotDbContext _db;
        private readonly NarfuApi _narfuApi;
        private readonly IVkApi _vkApi;
        private readonly OpenWeatherMapApi _weatherApi;

        public SendToConversationTasks(OpenWeatherMapApi weatherApi, NarfuApi narfuApi, IVkApi vkApi, BotDbContext db)
        {
            _weatherApi = weatherApi;
            _narfuApi = narfuApi;
            _vkApi = vkApi;
            _db = db;

            InitJobs();
        }

        public async Task SendToConv(long id, int group = 0, string city = "")
        {
            const int convId = 2000000000;
            id = convId + id;

            if(!string.IsNullOrWhiteSpace(city) && await _weatherApi.IsCityExists(city))
            {
                try
                {
                    Log.Information("Отправка погоды в беседу {0}", id);
                    var weather = await _weatherApi.GetDailyWeatherAt(city, DateTime.Today);
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        PeerId = id,
                        Message = $"Погода в городе {city} на сегодня ({DateTime.Now:dddd, dd.MM.yyyy}):\n{weather}"
                    });
                }
                catch(FlurlHttpException ex)
                {
                    Log.Error("openweathermap API недоступен (http code - {0}", ex.Call.HttpStatus);
                    var msg = "Невозможно получить погоду с сайта.";
                    await _vkApi.Messages.SendError(msg, id);
                }
                catch(Exception ex)
                {
                    Log.Fatal(ex, "Ошибка при получении погоды");
                    var msg = "Непредвиденная ошибка при получении погоды.";
                    await _vkApi.Messages.SendError(msg, id);
                }
            }

            if(_narfuApi.Students.IsCorrectGroup(group))
            {
                try
                {
                    Log.Information("Отправка расписания в беседу {0}", id);
                    var schedule = await _narfuApi.Students.GetScheduleAtDate(group, DateTime.Now);
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        PeerId = id,
                        Message = schedule.ToString()
                    });
                }
                catch(FlurlHttpException ex)
                {
                    Log.Error("ruz.narfu.ru недоступен (http code - {0}", ex.Call.HttpStatus);
                    var msg = $"Невозможно получить расписание с сайта (код ошибки - {ex.Call.HttpStatus}).";
                    await _vkApi.Messages.SendError(msg, id);
                }
                catch(Exception ex)
                {
                    Log.Fatal(ex, "Ошибка при получении расписания");
                    var msg = "Непредвиденнная ошибка при получении расписания с сайта.";
                    await _vkApi.Messages.SendError(msg, id);
                }
            }
        }

        public void InitJobs()
        {
            foreach(var job in _db.CronJobs)
            {
                RecurringJob.AddOrUpdate<SendToConversationTasks>(
                                                                  $"DAILY__{job.Name}",
                                                                  x => x.SendToConv(job.VkId, job.NarfuGroup,
                                                                                    job.WeatherCity),
                                                                  $"{job.Minutes} {job.Hours} * * 1-6",
                                                                  TimeZoneInfo.Local
                                                                 );
            }
        }

        public void Dummy()
        {
            //TODO: lol
        }
    }
}