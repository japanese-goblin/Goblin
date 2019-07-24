using MediatR;
using Vk.Models;

namespace Goblin.Bot.Notifications.GroupJoin
{
    public class GroupJoinNotification : INotification
    {
        public CallbackResponse Response { get; set; }
    }
}