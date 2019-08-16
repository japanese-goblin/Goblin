using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VkNet.Model;

namespace Goblin.Application.Commands
{
    public class DebugCommand : ITextCommand
    {
        public string Name => "Дебаг";
        public string Description => "Получить информацию о состоянии бота";
        public string Usage => Name;
        public bool IsAdminCommand => true;
        public string[] Aliases => new[] { "дебуг", "дебаг" };
        
        private readonly BotDbContext _db;

        public DebugCommand(BotDbContext db)
        {
            _db = db;
        }
        
        public Task<IResult> Execute(Message msg, BotUser user = null)
        {
            var strBuilder = new StringBuilder();
            
            var bday = new DateTime(2017, 4, 29, 19, 42, 0);
            var dis = DateTime.Now - bday;

            var startTime = Process.GetCurrentProcess().StartTime;
            var uptime = DateTime.Now - startTime;

            var users = _db.BotUsers.AsNoTracking().Count();
            var subscribes = _db.Subscribes.AsNoTracking();
            
            strBuilder.AppendFormat("Время старта: {0:F}", startTime).AppendLine();
            strBuilder.AppendFormat("Я работаю уже {0} часов {1} минут", uptime.Hours, uptime.Minutes)
                      .AppendLine();
            strBuilder.AppendFormat("Мне уже {0} дней ({1:dd.MM.yyyy})", dis.Days, bday)
                      .AppendLine().AppendLine();
            strBuilder.AppendFormat("Всего пользователей {0} ({1} расписание и {2} погода)",
                                    users, subscribes.Count(x => x.IsSchedule),
                                    subscribes.Count(x => x.IsWeather));

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = strBuilder.ToString()
            });
        }
    }
}