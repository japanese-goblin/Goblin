using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.BackgroundJobs.Jobs
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

        public async Task Execute()
        {
            Func<string, IEnumerable<long>, Task> vk = async (text, userIds) =>
            {
                await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                {
                    Message = text,
                    UserIds = userIds
                });
            };

            Func<string, IEnumerable<long>, Task> telegram = async (text, userIds) =>
            {
                foreach(var userId in userIds)
                {
                    try
                    {
                        await _botClient.SendTextMessageAsync(userId, text);
                    }
                    catch(ApiRequestException ex)
                    {
                        if(ex.ErrorCode == 403) // Forbidden: bot was blocked by the user
                        {
                            var user = _db.TgBotUsers.FirstOrDefault(x => x.Id == userId);
                            if(user != null)
                            {
                                _db.TgBotUsers.Remove(user);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, "Ошибка при отправке погоды");
                    }
                }

                await _db.SaveChangesAsync();
            };

            await SendSchedule<VkBotUser>(vk);
            await SendSchedule<TgBotUser>(telegram);
        }

        private async Task SendSchedule<T>(Func<string, IEnumerable<long>, Task> func) where T : BotUser
        {
            var grouped = _db.Set<T>()
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
                        await func(result.Message, ids);
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, "Ошибка при отправке расписания");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1.5));
                }
            }
        }
    }
}