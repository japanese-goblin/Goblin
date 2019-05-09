using System.Threading;
using System.Threading.Tasks;
using Goblin.Persistence;
using MediatR;
using Vk;

namespace Goblin.Bot.Notifications.Confirmation
{
    public class ConfirmationNotificationHandler : INotificationHandler<ConfirmationNotification>
    {
        private readonly VkApi _api;
        private readonly ApplicationDbContext _context;

        public ConfirmationNotificationHandler(VkApi api, ApplicationDbContext context)
        {
            _api = api;
            _context = context;
        }

        public async Task Handle(ConfirmationNotification notification, CancellationToken cancellationToken)
        {
            await _api.Messages.Send(_context.GetAdmins(), "Получен запрос на подтверждение адреса сервера");
        }
    }
}