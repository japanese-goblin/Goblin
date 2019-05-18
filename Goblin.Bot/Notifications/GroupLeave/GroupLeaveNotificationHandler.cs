using System.Threading;
using System.Threading.Tasks;
using Goblin.Persistence;
using MediatR;
using Vk;

namespace Goblin.Bot.Notifications.GroupLeave
{
    public class GroupLeaveNotificationHandler : INotificationHandler<GroupLeaveNotification>
    {
        private readonly VkApi _api;
        private readonly ApplicationDbContext _context;

        public GroupLeaveNotificationHandler(VkApi api, ApplicationDbContext context)
        {
            _api = api;
            _context = context;
        }

        public async Task Handle(GroupLeaveNotification notification, CancellationToken cancellationToken)
        {
            var userId = Vk.Models.Responses.GroupLeave.FromJson(notification.Response.Object.ToString()).UserId;
            var vkUser = await _api.Users.Get(userId);

            await _api.Messages.Send(_context.GetAdmins(), $"@id{userId} ({vkUser}) отписался");
        }
    }
}