using System;
using System.Collections.Generic;
using Goblin.Application.Core.Models;
using VkNet.Model.Keyboard;
using VkNet.Enums.SafetyEnums;

namespace Goblin.Application.Vk.Converters
{
    public static class KeyboardConverter
    {
        public static MessageKeyboard FromCoreToVk(CoreKeyboard coreKeyboard)
        {
            var kb = new KeyboardBuilder();
            kb.SetInline(coreKeyboard.IsInline);
            if(coreKeyboard.IsOneTime)
            {
                kb.SetOneTime();
            }

            foreach(var line in coreKeyboard.Buttons)
            {
                foreach(var button in line)
                {
                    var color = FromCoreColorToVk(button.Color);
                    kb.AddButton(button.Title, button.PayloadValue, color, button.PayloadKey);
                }

                kb.AddLine();
            }

            return kb.Build();
        }

        private static KeyboardButtonColor FromCoreColorToVk(CoreKeyboardButtonColor coreColor)
        {
            var dict = new Dictionary<CoreKeyboardButtonColor, KeyboardButtonColor>()
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
            else
            {
                throw new ArgumentException(nameof(coreColor));
            }
        }
    }
}