namespace Goblin.Models.Keyboard
{
    public sealed class ButtonColor
    {
        private readonly string name;

        public static readonly ButtonColor Default = new ButtonColor("default");
        public static readonly ButtonColor Primary = new ButtonColor("primary");
        public static readonly ButtonColor Negative = new ButtonColor("negative");
        public static readonly ButtonColor Positive = new ButtonColor("positive");

        public ButtonColor(string Name)
        {
            name = Name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}