using System;
using System.Threading.Tasks;
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
                        Message = weather.ToString()
                    });
                }
                catch
                {
                    //TODO: log
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
                catch
                {
                    //TODO: log
                }
            }
        }

        public void InitJobs()
        {
            foreach(var job in _db.CronJobs)
            {
                RecurringJob.AddOrUpdate<SendToConversationTasks>(
                                                         $"DAILY__{job.Name}",
                                                         x => x.SendToConv(job.VkId, job.NarfuGroup, job.WeatherCity),
                                                         $"{job.Minutes} {job.Hours} * * 1-6", TimeZoneInfo.Local
                                                        );
            }
        }

        public void Dummy()
        {
            //TODO: lol
        }
    }
}