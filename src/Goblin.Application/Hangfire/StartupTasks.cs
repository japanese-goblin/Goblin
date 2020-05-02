using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.Application.Options;
using Goblin.DataAccess;
using Hangfire;
using Microsoft.Extensions.Options;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class StartupTasks
    {
        private readonly BotDbContext _db;
        private readonly MailingOptions _options;
        private readonly IVkApi _vkApi;

        public StartupTasks(BotDbContext db, IVkApi vkApi, IOptions<MailingOptions> options)
        {
            _db = db;
            _vkApi = vkApi;
            _options = options.Value;
        }

        public void ConfigureHangfire()
        {
            ConfigureMailing();

            BackgroundJob.Enqueue<StartupTasks>(x => x.SendOldReminds());

            BackgroundJob.Enqueue<SendToConversationTasks>(x => x.Dummy());
            RecurringJob.AddOrUpdate<SendRemindTask>("SendRemind", x => x.SendRemind(),
                                                     Cron.Minutely, TimeZoneInfo.Local);
        }

        private void ConfigureMailing()
        {
            var scheduleSettings = _options.Schedule;
            var weatherSettings = _options.Weather;

            if(scheduleSettings.IsEnabled)
            {
                var scheduleTime = $"{scheduleSettings.Minute} {scheduleSettings.Hour} * * 1-6";
                RecurringJob.AddOrUpdate<ScheduleTask>("SendDailySchedule", x => x.SendSchedule(),
                                                       scheduleTime, TimeZoneInfo.Local);
            }

            if(weatherSettings.IsEnabled)
            {
                var weatherTime = $"{weatherSettings.Minute} {weatherSettings.Hour} * * *";
                RecurringJob.AddOrUpdate<WeatherTask>("SendDailyWeather", x => x.SendDailyWeather(),
                                                      weatherTime, TimeZoneInfo.Local);
            }
        }

        public async Task SendOldReminds()
        {
            var reminds = _db.Reminds.Where(x => x.Date < DateTime.Now);

            foreach(var remind in reminds)
            {
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    PeerId = remind.BotUserId,
                    Message = $"Напоминаю:\n{remind.Text}"
                });

                _db.Reminds.Remove(remind);
            }

            await _db.SaveChangesAsync();
        }
    }
}