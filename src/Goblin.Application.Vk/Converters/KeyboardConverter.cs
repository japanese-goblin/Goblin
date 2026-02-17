using System;
using System.Collections.Generic;
using Goblin.Application.Core.Models;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace Goblin.Application.Vk.Converters;

public static class KeyboardConverter
{
    public static MessageKeyboard FromCoreToVk(CoreKeyboard coreKeyboard, bool isInlineKeyboardAllowed = false)
    {
        if(coreKeyboard is null)
        {
            return null;
        }
        var kb = new KeyboardBuilder();
        var inlineKeyboardEnabled = coreKeyboard.IsInline && isInlineKeyboardAllowed;
        if(!isInlineKeyboardAllowed)
        {
            if(coreKeyboard.IsOneTime)
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

        foreach(var line in coreKeyboard.Buttons)
        {
            if(!isFirst)
            {
                kb.AddLine();
            }

            foreach(var button in line)
            {
                var color = FromCoreColorToVk(button.Color);
                kb.AddButton(new MessageKeyboardButtonAction()
                {
                    Label = button.Title,
                    Payload = button.Payload,
                    Type = inlineKeyboardEnabled ? KeyboardButtonActionType.Callback : KeyboardButtonActionType.Text,
                }, color);
                // kb.AddButton(button.Title, button.PayloadValue, color, button.PayloadKey);
            }

            isFirst = false;
        }

        return kb.Build();
    }

    private static KeyboardButtonColor FromCoreColorToVk(CoreKeyboardButtonColor coreColor)
    {
        var dict = new Dictionary<CoreKeyboardButtonColor, KeyboardButtonColor>
        {
            [CoreKeyboardButtonColor.Default] = KeyboardButtonColor.Default,
            [CoreKeyboardButtonColor.Negative] = KeyboardButtonColor.Negative,
            [CoreKeyboardButtonColor.Positive] = KeyboardButtonColor.Positive,
            [CoreKeyboardButtonColor.Primary] = KeyboardButtonColor.Primary
        };

        if(dict.TryGetValue(coreColor, out var result))
        {
            return result;
        }

        throw new ArgumentException(nameof(coreColor));
    }
}