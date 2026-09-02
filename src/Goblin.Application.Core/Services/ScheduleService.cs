using System.Text;
using Goblin.Application.Core.Extensions;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Services;

internal class ScheduleService(
    INarfuApi narfuApi,
    IDistributedCache distributedCache,
    TimeProvider timeProvider,
    ILogger<ScheduleService> logger) : IScheduleService
{
    private const string CachePrefix = "narfu_schedule";
    private static readonly TimeSpan LessonsCacheDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan ExamsCacheDuration = TimeSpan.FromHours(3);

    public async Task<CommandExecutionResult> GetSchedule(int narfuGroup, DateTime date, CancellationToken ct = default)
    {
        var group = narfuApi.Students.GetGroupByRealId(narfuGroup);
        if (group is null)
        {
            return CommandExecutionResult.Failed($"Группа {narfuGroup} не найдена");
        }

        var cacheKey = GetLessonsCacheKey(narfuGroup, date);
        var cachedData = await distributedCache.GetAsync<LessonsViewModel>(cacheKey, ct);
        if (cachedData is not null)
        {
            return BuildScheduleResult(cachedData.Lessons, date);
        }

        try
        {
            var lessons = await narfuApi.Students.GetScheduleAtDate(narfuGroup, date);
            await distributedCache.SetAsync(
                cacheKey,
                lessons,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = LessonsCacheDuration },
                ct);
            return BuildScheduleResult(lessons.Lessons, date);
        }
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException)
        {
            return CommandExecutionResult.Failed(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении расписания на день");
            return CommandExecutionResult.Failed(DefaultErrors.NarfuUnexpectedError);
        }
    }

    public async Task<CommandExecutionResult> GetExams(int narfuGroup, CancellationToken ct = default)
    {
        var group = narfuApi.Students.GetGroupByRealId(narfuGroup);
        if (group is null)
        {
            return CommandExecutionResult.Failed($"Группа {narfuGroup} не найдена");
        }

        var cacheKey = GetExamsCacheKey(narfuGroup);
        var cachedData = await distributedCache.GetAsync<ExamsViewModel>(cacheKey, ct);
        if (cachedData is not null)
        {
            return BuildExamsResult(cachedData.Lessons);
        }

        try
        {
            var exams = await narfuApi.Students.GetExams(narfuGroup);
            await distributedCache.SetAsync(
                cacheKey,
                exams,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ExamsCacheDuration },
                ct);
            return BuildExamsResult(exams.Lessons);
        }
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException)
        {
            return CommandExecutionResult.Failed(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении экзаменов");
            return CommandExecutionResult.Failed(DefaultErrors.NarfuUnexpectedError);
        }
    }

    private static CommandExecutionResult BuildScheduleResult(IReadOnlyCollection<Lesson> lessons, DateTime date)
    {
        if (lessons.Count == 0)
        {
            return CommandExecutionResult.Success($"На {date:dd.MM (dddd)} расписание отсутствует!");
        }

        var strBuilder = new StringBuilder();
        strBuilder.Append($"Расписание на {date:dd.MM (dddd)}:").AppendLine();

        foreach (var lesson in lessons.Where(p => p.StartTime.Date == date.Date))
        {
            strBuilder.Append($"{lesson.Number}) {lesson.StartEndTime} - {lesson.Name} ({lesson.Teacher}) [{lesson.Type}]")
                .AppendLine();

            if (!string.IsNullOrWhiteSpace(lesson.Groups))
            {
                strBuilder.Append($"У группы {lesson.Groups}").AppendLine();
            }

            strBuilder.Append($"В ауд. {lesson.Auditory} ({lesson.Address})")
                .AppendLine()
                .AppendLine();
        }

        return CommandExecutionResult.Success(strBuilder.ToString());
    }

    private CommandExecutionResult BuildExamsResult(IReadOnlyCollection<Lesson> lessons)
    {
        var now = timeProvider.GetLocalNow();
        var exams = lessons.Where(p => p.StartTime.Date > now).ToArray();
        if (exams.Length == 0)
        {
            return CommandExecutionResult.Success("На данный момент список экзаменов отсутствует");
        }

        var strBuilder = new StringBuilder();
        var grouped = exams.GroupBy(p => p.Name);

        foreach (var group in grouped)
        {
            var first = group.First();
            var last = group.Last();

            strBuilder.Append($"{first.StartTime:D}:").AppendLine();

            strBuilder.Append($"{first.StartTime:HH:mm}-{last.EndTime:HH:mm} - {first.Name} [{first.Type}] ({first.Teacher})")
                .AppendLine()
                .Append($"У группы {first.Groups}")
                .AppendLine()
                .Append($"В аудитории {first.Auditory} ({first.Address})")
                .AppendLine();

            strBuilder.AppendLine();
        }

        return CommandExecutionResult.Success(strBuilder.ToString());
    }

    private static string GetLessonsCacheKey(int narfuGroupId, DateTime date)
    {
        return $"{CachePrefix}:{narfuGroupId}:lessons:{date:dd_MM_yyyy}";
    }

    private static string GetExamsCacheKey(int narfuGroupId)
    {
        return $"{CachePrefix}:{narfuGroupId}:exams";
    }
}
