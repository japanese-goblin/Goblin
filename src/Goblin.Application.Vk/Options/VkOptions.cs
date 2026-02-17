using System.ComponentModel.DataAnnotations;

namespace Goblin.Application.Vk.Options;

public class VkOptions
{
    [Required]
    public string AccessToken { get; set; } = "";

    [Required]
    public string ConfirmationCode { get; set; } = "";

    [Required]
    public string SecretKey { get; set; } = "";
}