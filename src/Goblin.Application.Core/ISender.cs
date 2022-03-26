using System.Threading.Tasks;
using Goblin.Domain;

namespace Goblin.Application.Core;

public interface ISender
{
    ConsumerType ConsumerType { get; }
    Task Send(long chatId, string message);
}