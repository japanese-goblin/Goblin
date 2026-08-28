using System.ComponentModel.DataAnnotations;

namespace Goblin.Application.Vk.Options;

public class VkOptions
{
    [Required]
    public string AccessToken { get; set; } = "";
}
