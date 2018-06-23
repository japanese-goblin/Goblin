using System.Collections.Generic;
using Newtonsoft.Json;

namespace Goblin.Models
{
    public class Keyboard
    {
        [JsonProperty("one_time")]
        public bool OneTime = false;

        [JsonProperty("buttons")]
        public List<List<Button>> Buttons = new List<List<Button>>();

        public Keyboard() { }

        public Keyboard(List<List<Button>> buts)
        {
            Buttons = buts;
        }

        public void AddButton(byte row, Button but)
        {
            row = (byte) (row - 1);
            if(Buttons.Count < row+1)
                Buttons.Add(new List<Button>());
            Buttons[row].Add(but);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}