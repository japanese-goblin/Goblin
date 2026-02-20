using System.Text;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Narfu.Abstractions;
using Goblin.OpenWeatherMap.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Commands.Text;

public class ConversationSettingsCommand : ITextCommand
{
    public bool IsAdminCommand => false;

    public string[] Aliases => ["настройки", "настройка", "настроить"];

    private readonly IOpenWeatherMapApi _openWeatherMapApi;
    private readonly INarfuApi _narfuApi;
    private readonly BotDbContext _context;

    public ConversationSettingsCommand(IOpenWeatherMapApi openWeatherMapApi, INarfuApi narfuApi, BotDbContext context)
    {
        _openWeatherMapApi = openWeatherMapApi;
        _narfuApi = narfuApi;
        _context = context;
    }

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if(!msg.IsConversation)
        {
            return CommandExecutionResult.Failed("Команда доступна только в беседах");
        }

        if(msg.CommandParameters.Any(string.IsNullOrEmpty))
        {
            return await GenerateCronInfo(msg.ChatId, user.ConsumerType);
        }

        var parameters = msg.Text.Split(' ', 3)[1..];
        if(parameters.Length != 2)
        {
            return CommandExecutionResult.Failed("Команда принимает лишь два параметра (слова, разделенных пробелами)");
        }

        var whatToSet = parameters[0];
        var data = parameters[1];

        if(whatToSet.Contains("групп", StringComparison.InvariantCultureIgnoreCase))
        {
            return await SetGroup(msg.ChatId, data, user.ConsumerType);
        }

        if(whatToSet.Equals("город", StringComparison.InvariantCultureIgnoreCase))
        {
            return await SetCity(msg.ChatId, data, user.ConsumerType);
        }

        if(whatToSet.Equals("время", StringComparison.InvariantCultureIgnoreCase))
        {
            return await SetTime(msg.ChatId, data, user.ConsumerType);
        }

        if(whatToSet.Equals("убрать", StringComparison.InvariantCultureIgnoreCase))
        {
            return await RemoveMailing(msg.ChatId, data, user.ConsumerType);
        }

        return CommandExecutionResult.Failed("Данный параметр не поддерживается");
    }

    private async Task<CommandExecutionResult> GenerateCronInfo(long chatId, ConsumerType consumerType)
    {
        var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId &&
                                                                   x.ConsumerType == consumerType);
        if(job is null)
        {
            return CommandExecutionResult.Failed("Рассылка для данной беседы не настроена");
        }

        var strBuilder = new StringBuilder($"Информация по беседе #{chatId}:");
        strBuilder.AppendLine();

        if(job.NarfuGroup.HasValue)
        {
            strBuilder.AppendFormat("Группа САФУ: {0}", job.NarfuGroup);
        }
        else
        {
            strBuilder.Append("Группа для рассылки расписания не установлена");
        }

        strBuilder.AppendLine();

        if(!string.IsNullOrWhiteSpace(job.WeatherCity))
        {
            strBuilder.AppendFormat("Город: {0}", job.WeatherCity);
        }
        else
        {
            strBuilder.Append("Город для рассылки погоды не установлен");
        }

        strBuilder.AppendLine();

        strBuilder.AppendFormat("Время рассылки: {0}:{1}", job.Time.Hour, job.Time.Minute);

        return CommandExecutionResult.Success(strBuilder.ToString());
    }

    private async Task<CommandExecutionResult> SetTime(long chatId, string time, ConsumerType consumerType)
    {
        var splittedTime = time.Split(':');
        if(splittedTime.Length != 2)
        {
            return CommandExecutionResult.Failed("Укажите время в формате часы:минуты (например, 11:30)");
        }

        try
        {
            var cronTime = new CronTime(splittedTime[1], splittedTime[0]);
            var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId);
            if(job is null)
            {
                await _context.CronJobs.AddAsync(new CronJob(chatId.ToString(), chatId, 0, string.Empty,
                                                             cronTime, consumerType, CronType.Schedule));
            }
            else
            {
                job.SetCronTime(cronTime);
            }

            await _context.SaveChangesAsync();
            return CommandExecutionResult.Success($"Время успешно установлено на {time}");
        }
        catch(Exception e)
        {
            return CommandExecutionResult.Failed(e.Message);
        }
    }

    private async Task<CommandExecutionResult> SetCity(long chatId, string city, ConsumerType consumerType)
    {
        var isCityExist = await _openWeatherMapApi.IsCityExists(city);
        if(!isCityExist)
        {
            return CommandExecutionResult.Failed("Указанный город не найден");
        }

        var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId);
        if(job is null)
        {
            var time = new CronTime("0", "6");
            await _context.CronJobs.AddAsync(new CronJob(chatId.ToString(), chatId, 0, city, time, consumerType, CronType.Weather));
        }
        else
        {
            job.CronType |= CronType.Weather;
            job.SetWeatherCity(city);
        }

        await _context.SaveChangesAsync();

        return CommandExecutionResult.Success($"Город успешно установлен на {city}");
    }

    private async Task<CommandExecutionResult> SetGroup(long chatId, string group, ConsumerType consumerType)
    {
        if(!int.TryParse(group, out var intGroup))
        {
            return CommandExecutionResult.Failed("Укажите корректный номер группы.");
        }

        var isExists = _narfuApi.Students.IsCorrectGroup(intGroup);
        if(!isExists)
        {
            return CommandExecutionResult.Failed($"Группа с номером {intGroup} не найдена.");
        }

        var groupName = _narfuApi.Students.GetGroupByRealId(intGroup).Name;

        var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId);
        if(job is null)
        {
            var time = new CronTime("0", "6");
            await _context.CronJobs.AddAsync(new CronJob(chatId.ToString(), chatId, intGroup, string.Empty, time,
                                                         consumerType, CronType.Weather));
        }
        else
        {
            job.CronType |= CronType.Schedule;
            job.SetNarfuGroup(intGroup);
        }

        await _context.SaveChangesAsync();

        return CommandExecutionResult.Success($"Группа успешно установлена на {intGroup} ({groupName})");
    }

    private async Task<CommandExecutionResult> RemoveMailing(long chatId, string whatToRemove, ConsumerType consumerType)
    {
        if(whatToRemove.Contains("расписани", StringComparison.InvariantCultureIgnoreCase))
        {
            var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId &&
                                                                       x.ConsumerType == consumerType);
            job.SetNarfuGroup(0);
            job.CronType &= ~CronType.Schedule;
            await _context.SaveChangesAsync();

            return CommandExecutionResult.Success("Рассылка расписания удалена. Установите группу для возобновления рассылки.");
        }

        if(whatToRemove.Contains("погод", StringComparison.InvariantCultureIgnoreCase))
        {
            var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId &&
                                                                       x.ConsumerType == consumerType);
            job.SetWeatherCity(string.Empty);
            job.CronType &= ~CronType.Weather;
            await _context.SaveChangesAsync();

            return CommandExecutionResult.Success("Рассылка погоды удалена. Установите город для возобновления рассылки.");
        }

        return CommandExecutionResult.Failed($"Параметр '{whatToRemove}' отсутствует. Пожалуйста, прочитайте справку.");
    }
}