using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Extensions
{
    public static class MessageCategoryExtensions
    {
        public static long SendError(this IMessagesCategory msgCategory, string error, long peerId)
        {
            return msgCategory.Send(new MessagesSendParams
            {
                PeerId = peerId,
                Message = $"❌ Ошибка: {error}"
            });
        }
    }
}