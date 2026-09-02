using Goblin.Application.Core.Models;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace Goblin.Application.Vk.Converters;

public static class KeyboardConverter
{
    public static MessageKeyboard? FromCoreToVk(CoreKeyboard? coreKeyboard, bool isInlineKeyboardAllowed = false)
    {
        if (coreKeyboard is null)
        {
            return null;
        }

        var kb = new KeyboardBuilder();
        var inlineKeyboardEnabled = coreKeyboard.IsInline && isInlineKeyboardAllowed;
        if (!isInlineKeyboardAllowed)
        {
            if (coreKeyboard.IsOneTime)
            {
                kb.SetOneTime();
            }
        }
        else
        {
            coreKeyboard.RemoveReturnToMenuButton();
        }

        kb.SetInline(inlineKeyboardEnabled);

        var isFirst = true;

        foreach (var line in coreKeyboard.Buttons)
        {
            if (!isFirst)
            {
                kb.AddLine();
            }

            foreach (var button in line)
            {
                var color = FromCoreColorToVk(button.Color);
                kb.AddButton(new MessageKeyboardButtonAction
                {
                    Label = button.Title,
                    Payload = button.Payload,
                    Type = inlineKeyboardEnabled ? KeyboardButtonActionType.Callback : KeyboardButtonActionType.Text
                }, color);

                // kb.AddButton(button.Title, button.PayloadValue, color, button.PayloadKey);
            }

            isFirst = false;
        }

        return kb.Build();
    }

    private static KeyboardButtonColor FromCoreColorToVk(CoreKeyboardButtonColor coreColor)
    {
        return coreColor switch
        {
            CoreKeyboardButtonColor.Default => KeyboardButtonColor.Default,
            CoreKeyboardButtonColor.Primary => KeyboardButtonColor.Primary,
            CoreKeyboardButtonColor.Negative => KeyboardButtonColor.Negative,
            CoreKeyboardButtonColor.Positive => KeyboardButtonColor.Positive,
            CoreKeyboardButtonColor.Secondary => KeyboardButtonColor.Secondary,
            _ => throw new ArgumentOutOfRangeException(nameof(coreColor), coreColor, "Необработанное значение цвета кнопки")
        };
    }
}
