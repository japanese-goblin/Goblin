using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Commands.Text
{
    public class DebugCommand : ITextCommand
    {
        public bool IsAdminCommand => true;
        public string[] Aliases => new[] { "дебуг", "дебаг" };

        private readonly BotDbContext _db;

        public DebugCommand(BotDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            var strBuilder = new StringBuilder();

            var birthday = new DateTime(2017, 4, 29, 19, 42, 0);
            var dis = DateTime.Now - birthday;

            var startTime = Process.GetCurrentProcess().StartTime;
            var uptime = DateTime.Now - startTime;

            var tgUsers = await _db.TgBotUsers.AsNoTracking().ToArrayAsync();

            var vkUsers = await _db.VkBotUsers.AsNoTracking().ToArrayAsync();

            var tgSchedule = tgUsers.Count(x => x.HasScheduleSubscription);
            var vkSchedule = vkUsers.Count(x => x.HasScheduleSubscription);

            var tgWeather = tgUsers.Count(x => x.HasWeatherSubscription);
            var vkWeather = vkUsers.Count(x => x.HasWeatherSubscription);

            var sumAll = tgUsers.Length + vkUsers.Length;
            var sumSchedule = tgSchedule + vkSchedule;
            var sumWeather = tgWeather + vkWeather;

            strBuilder.AppendFormat("Время старта: {0:F}", startTime).AppendLine()
                      .AppendFormat("Я работаю уже {0} часов {1} минут", uptime.Hours, uptime.Minutes)
                      .AppendLine()
                      .AppendFormat("Мне уже {0} дней ({1:dd.MM.yyyy})", dis.Days, birthday)
                      .AppendLine().AppendLine()
                      .AppendFormat("Всего пользователей {0} ({1} вк и {2} телеграм)",
                                    sumAll, vkUsers.Length, tgUsers.Length)
                      .AppendLine()
                      .AppendLine("Подписки:")
                      .AppendFormat("• Погода - {0} ({1} вк, {2} телеграм)", sumWeather, vkWeather, tgWeather)
                      .AppendLine()
                      .AppendFormat("• Расписание - {0} ({1} вк, {2} телеграм)", sumSchedule, vkSchedule, tgSchedule);

            return new SuccessfulResult
            {
                Message = strBuilder.ToString()
            };
        }
    }
}