using Goblin.Application.Core.Models;
using Goblin.Domain;

namespace Goblin.Application.Core;

public interface ISender
{
    int TextLimit { get; }
    ConsumerType ConsumerType { get; }
    Task Send(long chatId, string message, CoreKeyboard keyboard = null, IEnumerable<string> attachments = null);
    Task SendToMany(IEnumerable<long> chatIds, string message, CoreKeyboard keyboard = null, IEnumerable<string> attachments = null);
}