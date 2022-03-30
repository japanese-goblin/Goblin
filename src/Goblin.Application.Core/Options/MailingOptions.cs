namespace Goblin.Application.Core.Options;

public class MailingOptions
{
    public bool IsVacations { get; set; }
    public MailingSettings Schedule { get; set; }
    public MailingSettings Weather { get; set; }
}