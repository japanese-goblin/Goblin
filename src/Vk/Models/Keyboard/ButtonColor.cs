namespace Vk.Models.Keyboard
{
    public sealed class ButtonColor
    {
        private readonly string _name;

        public static readonly ButtonColor Default = new ButtonColor("default");
        public static readonly ButtonColor Primary = new ButtonColor("primary");
        public static readonly ButtonColor Negative = new ButtonColor("negative");
        public static readonly ButtonColor Positive = new ButtonColor("positive");

        public ButtonColor(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}