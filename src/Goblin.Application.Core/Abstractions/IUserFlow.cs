using Goblin.Domain;

namespace Goblin.Application.Core.Abstractions;

public interface IUserFlow
{
    string PayloadKey { get; }

    FlowType Type { get; }

    Task<FlowExecutionResult> HandleAsync(
        UserFlowContext context,
        CancellationToken cancellationToken);
}
