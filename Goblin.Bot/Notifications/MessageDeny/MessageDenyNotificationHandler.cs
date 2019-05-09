using System.Threading;
using System.Threading.Tasks;
using Goblin.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vk;

namespace Goblin.Bot.Notifications.MessageDeny
{
    public class MessageDenyNotificationHandler : INotificationHandler<MessageDenyNotification>
    {
        private readonly VkApi _api;
        private readonly ApplicationDbContext _context;

        public MessageDenyNotificationHandler(VkApi api, ApplicationDbContext context)
        {
            _api = api;
            _context = context;
        }

        public async Task Handle(MessageDenyNotification notification, CancellationToken cancellationToken)
        {
            var userId = Vk.Models.Responses.GroupLeave.FromJson(notification.Response.Object.ToString()).UserId;
            var botUser = await _context.BotUsers.FirstOrDefaultAsync(x => x.Vk == userId,
                                                                      cancellationToken);

            if(botUser != null)
            {
                _context.BotUsers.Remove(botUser);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var userName = await _api.Users.Get(userId);
            await _api.Messages.Send(_context.GetAdmins(), $"@id{userId} ({userName}) запретил сообщения");
        }
    }
}