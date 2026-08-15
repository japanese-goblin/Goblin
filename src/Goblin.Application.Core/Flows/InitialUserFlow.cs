using Goblin.Domain;

namespace Goblin.Application.Core.Flows;

public class InitialUserFlow : IUserFlow
{
    public string Name => "Первый запуск";
    public FlowType Type => FlowType.Start;

    public Task<FlowExecutionResult> HandleAsync(UserFlowContext context, CancellationToken cancellationToken)
    {
        var response = new FlowExecutionResult(FlowType.MainMenu, null, true, "Вы перешли в главное меню", null);
        return Task.FromResult(response);
    }
}