using MediatR;
using Vk.Models;

namespace Goblin.Bot.Notifications.GroupLeave
{
    public class GroupLeaveNotification : INotification
    {
        public CallbackResponse Response { get; set; }
    }
}