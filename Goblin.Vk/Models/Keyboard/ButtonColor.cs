namespace Goblin.Vk.Models
{
    public sealed class ButtonColor
    {
        private readonly string name;

        public static readonly ButtonColor Default = new ButtonColor("default");
        public static readonly ButtonColor Primary = new ButtonColor("primary");
        public static readonly ButtonColor Negative = new ButtonColor("negative");
        public static readonly ButtonColor Positive = new ButtonColor("positive");

        public ButtonColor(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}