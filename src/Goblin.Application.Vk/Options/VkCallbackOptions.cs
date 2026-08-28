using System.ComponentModel.DataAnnotations;

namespace Goblin.Application.Vk.Options;

public class VkCallbackOptions
{
    [Required]
    public string ConfirmationCode { get; set; } = "";

    [Required]
    public string SecretKey { get; set; } = "";
}
