﻿using System;
using System.Collections.Generic;
using Goblin.Application.Core.Models;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Vk.Converters;

public static class KeyboardConverter
{
    public static MessageKeyboard FromCoreToVk(CoreKeyboard coreKeyboard, bool isInlineKeyboardAllowed = false)
    {
        var kb = new KeyboardBuilder();
        var isInlineKeyboardEnabled = coreKeyboard.IsInline && isInlineKeyboardAllowed;
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

        kb.SetInline(isInlineKeyboardEnabled);

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
                kb.AddButton(button.Title, button.PayloadValue, color, button.PayloadKey);
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