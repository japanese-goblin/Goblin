namespace Goblin.Application.Core.Models;

public class CoreKeyboardButton
{
    public string Title { get; set; }
    public string Payload { get; set; }
    public CoreKeyboardButtonColor Color { get; set; }

    public string PayloadKey { get; set; }
    public string PayloadValue { get; set; }

    public void SetPayload(string key, string value)
    {
        Payload = $"{{\"{key}\":\"{value}\"}}";
        PayloadKey = key;
        PayloadValue = value;
    }
    
    public string GetPayload() => Payload = $"{{\"{PayloadKey}\":\"{PayloadValue}\"}}";
}