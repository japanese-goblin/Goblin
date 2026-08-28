using Goblin.Domain;

namespace Goblin.Application.Core.Commands.Merged;

public class StartCommand : ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => ["старт", "начать", "/start"];

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        user.Session.FlowStepType = null;
        if(user.Session.FlowType == FlowType.Start)
        {
            return Task.FromResult(CommandExecutionResult.Success(
                "Добро пожаловать! 👺\nПеред началом работы настройте группу САФУ и город для прогноза погоды.",
                DefaultKeyboards.GetInitializationKeyboard(user)));
        }

        user.Session.FlowType = FlowType.MainMenu;
        return Task.FromResult(CommandExecutionResult.Success("Воспользуйтесь клавиатурой для управления ботом:", DefaultKeyboards.GetMainMenuKeyboard()));
    }
}
