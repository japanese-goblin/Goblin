using System.Threading;
using System.Threading.Tasks;
using Goblin.Persistence;
using MediatR;
using Vk;

namespace Goblin.Bot.Notifications.GroupJoin
{
    public class GroupJoinNotificationHandler : INotificationHandler<GroupJoinNotification>
    {
        private readonly VkApi _api;
        private readonly ApplicationDbContext _context;

        public GroupJoinNotificationHandler(VkApi api, ApplicationDbContext context)
        {
            _api = api;
            _context = context;
        }

        public async Task Handle(GroupJoinNotification notification, CancellationToken cancellationToken)
        {
            var join = Vk.Models.Responses.GroupJoin.FromJson(notification.Response.Object.ToString());
            var userName = await _api.Users.Get(join.UserId);

            await _api.Messages.Send(_context.GetAdmins(), $"@id{join.UserId} ({userName}) подписался");
        }
    }
}