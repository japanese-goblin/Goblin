using Vk.Models.Keyboard;

namespace Goblin.Data.Models
{
    public class CommandResponse
    {
        public string Text { get; set; }
        public Keyboard Keyboard { get; set; }
        public string[] Attachments { get; set; }
    }
}