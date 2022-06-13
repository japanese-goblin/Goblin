using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
using Goblin.Domain;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

namespace Goblin.BackgroundJobs.Jobs;

public class StartupTasks
{
    private readonly BotDbContext _db;
    private readonly IVkApi _api;
    private readonly MailingOptions _options;

    public StartupTasks(BotDbContext db, IOptions<MailingOptions> options, IVkApi api)
    {
        _db = db;
        _api = api;
        _options = options.Value;

        InitJobsFromDatabase();
    }

    public void ConfigureHangfire()
    {
        ConfigureMailing();

        BackgroundJob.Enqueue<SendRemindTasks>(x => x.SendOldRemindsOnStartup());

        RecurringJob.AddOrUpdate<SendRemindTasks>("SendRemind", x => x.SendRemindEveryMinute(),
                                                  Cron.Minutely, TimeZoneInfo.Local);
    }

    public async Task RemoveInactiveUsersFromVk()
    {
        const int count = 200;
        var today = DateTime.Today;
        var inactiveTime = TimeSpan.FromDays(75);

        var vkUsers = await _db.BotUsers.Where(x => x.ConsumerType == ConsumerType.Vkontakte).ToArrayAsync();
        var getConversationResult = await _api.Messages.GetConversationsAsync(new GetConversationsParams
        {
            Count = 1
        });

        var conversationsCount = (getConversationResult.Count / 200) + 1;

        for(var offset = 0; offset < conversationsCount; offset++)
        {
            var conversations = await _api.Messages.GetConversationsAsync(new GetConversationsParams
            {
                Count = count,
                Offset = (ulong) (count * offset),
                Filter = GetConversationFilter.All
            });

            var inactiveUsers = conversations.Items
                                             .Where(x => !x.Conversation.CanWrite.Allowed ||
                                                         today - x.LastMessage.Date > inactiveTime)
                                             .Select(x => x.Conversation.Peer.Id)
                                             .ToArray();

            var usersToRemove = vkUsers.Where(x => inactiveUsers.Contains(x.Id));
            _db.BotUsers.RemoveRange(usersToRemove);
        }

        await _db.SaveChangesAsync();
    }

    private void ConfigureMailing()
    {
        var scheduleSettings = _options.Schedule ?? new MailingSettings();
        var weatherSettings = _options.Weather ?? new MailingSettings();

        if(scheduleSettings.IsEnabled)
        {
            var scheduleTime = $"{scheduleSettings.Minute} {scheduleSettings.Hour} * * 1-6";
            RecurringJob.AddOrUpdate<ScheduleTask>("DailySchedule", x => x.Execute(),
                                                   scheduleTime, TimeZoneInfo.Local);
        }

        if(weatherSettings.IsEnabled)
        {
            var weatherTime = $"{weatherSettings.Minute} {weatherSettings.Hour} * * *";
            RecurringJob.AddOrUpdate<WeatherTask>("DailyWeather", x => x.Execute(),
                                                  weatherTime, TimeZoneInfo.Local);
        }
    }

    private void InitJobsFromDatabase()
    {
        foreach(var job in _db.CronJobs.ToArray())
        {
            RecurringJob.AddOrUpdate<SendToChatTasks>(
                                                      $"{job.ConsumerType}__{job.Name}",
                                                      x => x.Execute(job.ChatId, job.ConsumerType, job.CronType,
                                                                     job.WeatherCity, job.NarfuGroup, job.Text),
                                                      job.Time.ToString(),
                                                      TimeZoneInfo.Local
                                                     );
        }
    }
}