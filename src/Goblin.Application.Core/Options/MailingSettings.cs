using System.ComponentModel.DataAnnotations;

namespace Goblin.Application.Core.Options;

public class MailingSettings
{
    [Range(0, 24)]
    public int Hour { get; set; }

    [Range(0, 60)]
    public int Minute { get; set; }

    public bool IsEnabled { get; set; }
}