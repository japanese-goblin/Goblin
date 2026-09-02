namespace Goblin.Application.Core.Options;

public class MailingSettings
{
    public bool IsEnabled { get; set; }

    public required string CronExpression { get; set; }
}
