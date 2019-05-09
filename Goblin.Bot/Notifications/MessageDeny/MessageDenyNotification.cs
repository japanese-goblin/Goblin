using MediatR;
using Vk.Models;

namespace Goblin.Bot.Notifications.MessageDeny
{
    public class MessageDenyNotification : INotification
    {
        public CallbackResponse Response { get; set; }
    }
}