using Goblin.Application.Core.Options;
using Goblin.BackgroundJobs.Jobs;
using Microsoft.Extensions.Options;
using Quartz;

namespace Goblin.BackgroundJobs.Options;

internal class ConfigureQuartzOptions(IOptions<MailingOptions> mailingOptions) : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions q)
    {
        q.AddJob<ResetUsersGroupsJob>(p =>
            p.WithIdentity(ResetUsersGroupsJob.JobKey));
        q.AddTrigger(p =>
            p.ForJob(ResetUsersGroupsJob.JobKey).StartNow());

        q.AddJob<SendRemindsJob>(p =>
            p.WithIdentity(SendRemindsJob.JobKey));
        q.AddTrigger(p =>
            p.ForJob(SendRemindsJob.JobKey)
                .WithCronSchedule("0 * * ? * *"));

        if (mailingOptions.Value.Schedule.IsEnabled)
        {
            q.AddJob<SendSchedulesJob>(p =>
                p.WithIdentity(SendSchedulesJob.JobKey));
            q.AddTrigger(p =>
                p.ForJob(SendSchedulesJob.JobKey)
                    .WithCronSchedule(mailingOptions.Value.Schedule.CronExpression));
        }

        if (mailingOptions.Value.Weather.IsEnabled)
        {
            q.AddJob<SendWeatherForecastJob>(p =>
                p.WithIdentity(SendWeatherForecastJob.JobKey));
            q.AddTrigger(p =>
                p.ForJob(SendWeatherForecastJob.JobKey)
                    .WithCronSchedule(mailingOptions.Value.Weather.CronExpression));
        }
        
        // TODO: добавить триггеры для джобов из db.CronJobs
    }
}