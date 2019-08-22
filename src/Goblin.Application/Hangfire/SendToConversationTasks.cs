using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using Hangfire;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class SendToConversationTasks
    {
        private readonly OpenWeatherMapApi _weatherApi;
        private readonly NarfuApi _narfuApi;
        private readonly IVkApi _vkApi;
        private readonly BotDbContext _db;

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
                    var weather = await _weatherApi.GetDailyWeatherAt(city, DateTime.Today);
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        PeerId = id,
                        Message = $"Погода в городе {city} на сегодня ({DateTime.Now:dddd, dd.MM.yyyy}):\n{weather}"
                    });
                }
                catch
                {
                    var msg = "Невозможно получить погоду с сайта.";
                    await _vkApi.Messages.SendError(msg, id);
                }
            }

            if(_narfuApi.Students.IsCorrectGroup(group))
            {
                try
                {
                    var schedule = await _narfuApi.Students.GetScheduleAtDate(group, DateTime.Now);
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        PeerId = id,
                        Message = schedule.ToString()
                    });
                }
                catch(FlurlHttpException ex)
                {
                    var msg = $"Невозможно получить расписание с сайта (код ошибки - {ex.Call.HttpStatus}).";
                    await _vkApi.Messages.SendError(msg, id);
                }
                catch(Exception)
                {
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