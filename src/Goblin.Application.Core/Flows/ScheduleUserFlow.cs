using Goblin.Application.Core.Extensions;
using Goblin.Domain;

namespace Goblin.Application.Core.Flows;

public class ScheduleUserFlow(IScheduleService api) : IUserFlow
{
    public string PayloadKey => PayloadType.Schedule.GetEnumMemberValue();

    public FlowType Type => FlowType.Schedule;

    public async Task<FlowExecutionResult> HandleAsync(UserFlowContext context, CancellationToken cancellationToken)
    {
        if (!context.User.NarfuGroup.HasValue)
        {
            return new FlowExecutionResult(
                FlowType.MainMenu,
                null,
                false,
                "Отсутствует группа САФУ - укажите её в настройках",
                null);
        }

        if (context.Message.ParsedPayload is null || !context.Message.ParsedPayload.TryGetValue(PayloadKey, out var scheduleDateInput))
        {
            return new FlowExecutionResult(
                FlowType.MainMenu,
                null,
                false,
                "Воспользуйтесь клавиатурой для управления ботом:",
                DefaultKeyboards.GetMainMenuKeyboard());
        }

        if (!DateTime.TryParse(scheduleDateInput, out var scheduleDate))
        {
            return new FlowExecutionResult(
                FlowType.Schedule,
                null,
                false,
                "Указана некорректная дата",
                DefaultKeyboards.GetScheduleKeyboard());
        }

        var getScheduleResponse = await api.GetSchedule(context.User.NarfuGroup.Value, scheduleDate, cancellationToken);
        return new FlowExecutionResult(
            FlowType.Schedule,
            null,
            getScheduleResponse.IsSuccessful,
            getScheduleResponse.Message,
            DefaultKeyboards.GetScheduleKeyboard());
    }
}
