using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using Hangfire;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk.Hangfire
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
            if(!string.IsNullOrWhiteSpace(city) && await _weatherApi.IsCityExists(city))
            {
                Log.Information("Отправка погоды в {0}", id);
                var weather = await _weatherApi.GetDailyWeatherWithResult(city, DateTime.Today);
                if(weather is FailedResult failed)
                {
                    await _vkApi.Messages.SendError(failed.Message, id);
                }
                else
                {
                    var success = weather as SuccessfulResult;
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        PeerId = id,
                        Message = success.Message
                    });
                }
            }

            if(_narfuApi.Students.IsCorrectGroup(group) && DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            {
                Log.Information("Отправка расписания в {0}", id);
                var schedule = await _narfuApi.Students.GetScheduleAtDateWithResult(group, DateTime.Now);
                if(schedule is FailedResult failed)
                {
                    await _vkApi.Messages.SendError(failed.Message, id);
                }
                else
                {
                    var success = schedule as SuccessfulResult;
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        PeerId = id,
                        Message = success.Message
                    });
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
                                                                  $"{job.Minutes} {job.Hours} * * *",
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