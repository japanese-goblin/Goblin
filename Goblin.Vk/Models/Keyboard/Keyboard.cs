using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Goblin.Vk.Models
{
    public class Keyboard
    {
        [JsonProperty("one_time")] public bool IsOneTime;

        [JsonProperty("buttons")] public List<List<Button>> Buttons = new List<List<Button>>();

        public Keyboard(bool isOneTime)
        {
            IsOneTime = isOneTime;
            AddLine();
        }

        public void AddButton(Button but)
        {
            Buttons.Last().Add(but);
        }

        public void AddButton(string Title, ButtonColor color, string cmd, string param)
        {
            Buttons.Last().Add(new Button(Title, color, cmd, param));
        }

        public void AddLine()
        {
            Buttons.Add(new List<Button>());
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}