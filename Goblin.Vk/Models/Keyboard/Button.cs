using System.Collections.Generic;
using Newtonsoft.Json;

namespace Goblin.Vk.Models
{
    public class Button
    {
        [JsonProperty("action")] public Dictionary<string, string> Action = new Dictionary<string, string>
        {
            ["type"] = "text"
        };

        [JsonProperty("color")] public string Color => _color.ToString();
        private readonly ButtonColor _color;

        public Button(string label, ButtonColor color, string cmd, string param)
        {
            _color = color;
            Action.Add("label", label);
            Action.Add("payload", $"{{\"{cmd}\":\"{param}\"}}");
        }
    }
}