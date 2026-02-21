using Goblin.Domain;

namespace Goblin.Application.Core;

public interface ISender
{
    int TextLimit { get; }

    ConsumerType ConsumerType { get; }

    Task Send(long chatId, string message, CoreKeyboard? keyboard = null, IReadOnlyCollection<string>? attachments = null);

    Task SendToMany(IReadOnlyCollection<long> chatIds, string message, CoreKeyboard? keyboard = null,
                    IReadOnlyCollection<string>? attachments = null);
}