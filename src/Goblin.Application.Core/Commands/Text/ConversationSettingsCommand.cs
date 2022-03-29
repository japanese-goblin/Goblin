using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Abstractions;
using Goblin.Domain.Entities;
using Goblin.Narfu.Abstractions;
using Goblin.OpenWeatherMap.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Commands.Text
{
    public class ConversationSettingsCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "настройки", "настройка", "настроить" };

        private readonly IOpenWeatherMapApi _openWeatherMapApi;
        private readonly INarfuApi _narfuApi;
        private readonly BotDbContext _context;

        public ConversationSettingsCommand(IOpenWeatherMapApi openWeatherMapApi, INarfuApi narfuApi, BotDbContext context)
        {
            _openWeatherMapApi = openWeatherMapApi;
            _narfuApi = narfuApi;
            _context = context;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            if(!msg.IsConversation)
            {
                return new FailedResult("Команда доступна только в беседах");
            }

            if(msg.CommandParameters.Any(string.IsNullOrEmpty))
            {
                return await GenerateCronInfo(msg.ChatId, user.ConsumerType);
            }

            var parameters = msg.Text.Split(' ', 3)[1..];
            if(parameters.Length != 2)
            {
                return new FailedResult("Команда принимает лишь два параметра (слова, разделенных пробелами)");
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

            return new FailedResult("Данный параметр не поддерживается");
        }

        private async Task<IResult> GenerateCronInfo(long chatId, ConsumerType consumerType)
        {
            var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId &&
                                                                       x.ConsumerType == consumerType);
            if(job is null)
            {
                return new FailedResult("Рассылка для данной беседы не настроена");
            }

            var strBuilder = new StringBuilder($"Информация по беседе #{chatId}:");
            strBuilder.AppendLine();

            if(job.NarfuGroup != 0)
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

            return new SuccessfulResult(strBuilder.ToString());
        }

        private async Task<IResult> SetTime(long chatId, string time, ConsumerType consumerType)
        {
            var splittedTime = time.Split(':');
            if(splittedTime.Length != 2)
            {
                return new FailedResult("Укажите время в формате часы:минуты (например, 11:30)");
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
                return new SuccessfulResult($"Время успешно установлено на {time}");
            }
            catch(Exception e)
            {
                return new FailedResult(e.Message);
            }
        }

        private async Task<IResult> SetCity(long chatId, string city, ConsumerType consumerType)
        {
            var isCityExist = await _openWeatherMapApi.IsCityExists(city);
            if(!isCityExist)
            {
                return new FailedResult("Указанный город не найден");
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

            return new SuccessfulResult($"Город успешно установлен на {city}");
        }

        private async Task<IResult> SetGroup(long chatId, string group, ConsumerType consumerType)
        {
            if(!int.TryParse(group, out var intGroup))
            {
                return new FailedResult("Укажите корректный номер группы.");
            }

            var isExists = _narfuApi.Students.IsCorrectGroup(intGroup);
            if(!isExists)
            {
                return new FailedResult($"Группа с номером {intGroup} не найдена.");
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

            return new SuccessfulResult($"Группа успешно установлена на {intGroup} ({groupName})");
        }

        private async Task<IResult> RemoveMailing(long chatId, string whatToRemove, ConsumerType consumerType)
        {
            if(whatToRemove.Contains("расписани", StringComparison.InvariantCultureIgnoreCase))
            {
                var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId &&
                                                                           x.ConsumerType == consumerType);
                job.SetNarfuGroup(0);
                job.CronType &= ~CronType.Schedule;
                await _context.SaveChangesAsync();

                return new SuccessfulResult("Рассылка расписания удалена. Установите группу для возобновления рассылки.");
            }

            if(whatToRemove.Contains("погод", StringComparison.InvariantCultureIgnoreCase))
            {
                var job = await _context.CronJobs.FirstOrDefaultAsync(x => x.ChatId == chatId &&
                                                                           x.ConsumerType == consumerType);
                job.SetWeatherCity(string.Empty);
                job.CronType &= ~CronType.Weather;
                await _context.SaveChangesAsync();

                return new SuccessfulResult("Рассылка погоды удалена. Установите город для возобновления рассылки.");
            }

            return new FailedResult($"Параметр '{whatToRemove}' отсутствует. Пожалуйста, прочитайте справку.");
        }
    }
}