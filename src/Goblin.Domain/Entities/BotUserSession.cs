namespace Goblin.Domain.Entities;

/// <summary>
///     Сессия взаимодействия с пользователем
/// </summary>
public class BotUserSession
{
    /// <summary>
    ///     ИД сессии
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     ИД пользователя
    /// </summary>
    public Guid BotUserId { get; set; }

    /// <summary>
    ///     Тип состояния
    /// </summary>
    public FlowType FlowType { get; set; }

    /// <summary>
    ///     Тип шага внутри состояния
    /// </summary>
    public string? FlowStepType { get; set; } = null!;

    /// <summary>
    ///     Пользователь
    /// </summary>
    public BotUser BotUser { get; set; } = null!;
}
