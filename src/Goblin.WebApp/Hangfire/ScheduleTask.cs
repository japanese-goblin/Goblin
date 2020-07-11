using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Extensions;
using Goblin.Application.Core.Options;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Vk.Extensions;
using Goblin.Application.Vk.Hangfire;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Hangfire
{
    public class ScheduleTask
    {
        private readonly TelegramBotClient _botClient;
        private readonly BotDbContext _db;
        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly IVkApi _vkApi;
        private readonly MailingOptions _mailingOptions;

        public ScheduleTask(BotDbContext db, IVkApi vkApi, IScheduleService scheduleService, TelegramBotClient botClient,
                            IOptions<MailingOptions> mailingOptions)
        {
            _db = db;
            _vkApi = vkApi;
            _scheduleService = scheduleService;
            _botClient = botClient;
            _logger = Log.ForContext<ScheduleTask>();
            _mailingOptions = mailingOptions.Value;
        }

        public async Task SendSchedule()
        {
            await SendToVk();
            await SendToTelegram();
        }

        private async Task SendToVk()
        {
            var grouped = _db.VkBotUsers
                             .AsNoTracking()
                             .Where(x => x.HasScheduleSubscription)
                             .ToArray()
                             .GroupBy(x => x.NarfuGroup);
            foreach(var group in grouped)
            {
                var result = await _scheduleService.GetSchedule(group.Key, DateTime.Today);
                if(!result.IsSuccessful && _mailingOptions.IsVacations)
                {
                    continue;
                }

                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id);
                        await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                        {
                            UserIds = ids,
                            Message = result.Message
                        });

                        await Task.Delay(TimeSpan.FromSeconds(1.5));
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, "Ошибка при отправке расписания вконтакте");
                    }
                }
            }
        }

        private async Task SendToTelegram()
        {
            var grouped = _db.TgBotUsers
                             .AsNoTracking()
                             .Where(x => x.HasScheduleSubscription)
                             .ToArray()
                             .GroupBy(x => x.NarfuGroup);
            foreach(var group in grouped)
            {
                try
                {
                    var result = await _scheduleService.GetSchedule(group.Key, DateTime.Today);
                    if(!result.IsSuccessful && _mailingOptions.IsVacations)
                    {
                        continue;
                    }
                    foreach(var user in group)
                    {
                        await _botClient.SendTextMessageAsync(user.Id, result.Message);
                    }
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Ошибка при отправке расписания telegram");
                }
            }
        }
    }
}