using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Extensions
{
    public static class KeyboardBuilderExtensions
    {
        public static KeyboardBuilder AddReturnToMenuButton(this KeyboardBuilder kb, bool addNewLine = true)
        {
            if(addNewLine)
            {
                kb.AddLine();
            }

            kb.AddButton("Вернуться в главное меню", "start",
                         KeyboardButtonColor.Default, "command");
            return kb;
        }
    }
}