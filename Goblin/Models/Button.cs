using System.Collections.Generic;
using Newtonsoft.Json;

namespace Goblin.Models
{
    public class Button
    {
        [JsonProperty("action")] public Dictionary<string, string> Action = new Dictionary<string, string>
        {
            ["type"] = "text"
        };

        [JsonProperty("color")] public string Color = "primary";

        public Button(string label, string payload = "")
        {
            Action.Add("label", label);
            Action.Add("payload", $"{{\"command\":\"{payload}\"}}");
        }
    }
}