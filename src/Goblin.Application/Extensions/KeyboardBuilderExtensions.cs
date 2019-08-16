using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Extensions
{
    public static class KeyboardBuilderExtensions
    {
        public static KeyboardBuilder AddReturnToMenuButton(this KeyboardBuilder kb)
        {
            kb.AddLine();

            kb.AddButton("Вернуться в главное меню", "command",
                         KeyboardButtonColor.Default, "start");
            return kb;
        }
    }
}