using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Hangfire;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk.Hangfire
{
    public class SendToConversationTasks
    {
        private readonly BotDbContext _db;
        private readonly ScheduleCommand _scheduleCommand;
        private readonly IVkApi _vkApi;
        private readonly WeatherDailyCommand _weatherDailyCommand;

        public SendToConversationTasks(ScheduleCommand scheduleCommand, WeatherDailyCommand weatherDailyCommand, IVkApi vkApi,
                                       BotDbContext db)
        {
            _scheduleCommand = scheduleCommand;
            _weatherDailyCommand = weatherDailyCommand;
            _vkApi = vkApi;
            _db = db;

            InitJobs();
        }

        public async Task SendToConv(long id, int group = 0, string city = "")
        {
            if(!string.IsNullOrWhiteSpace(city))
            {
                Log.Information("Отправка погоды в {0}", id);
                var result = await _weatherDailyCommand.GetDailyWeather(city, DateTime.Today);

                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    PeerId = id,
                    Message = result.Message
                });
            }

            if(DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            {
                Log.Information("Отправка расписания в {0}", id);
                var result = await _scheduleCommand.GetSchedule(group, DateTime.Now);
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    PeerId = id,
                    Message = result.Message
                });
            }
        }

        public void InitJobs()
        {
            foreach(var job in _db.CronJobs)
            {
                RecurringJob.AddOrUpdate<SendToConversationTasks>(
                                                                  $"DAILY__{job.Name}",
                                                                  x => x.SendToConv(job.ChatId, job.NarfuGroup,
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