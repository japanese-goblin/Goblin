using System.Collections.Generic;
using Goblin.Application.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Goblin.Application.Telegram.Converters
{
    public static class KeyboardConverter
    {
        public static IReplyMarkup FromCoreToTg(CoreKeyboard coreKeyboard)
        {
            var keyboard = new ReplyKeyboardMarkup
            {
                OneTimeKeyboard = coreKeyboard.IsOneTime,
                ResizeKeyboard = true
            };
            var tgButtonsList = new List<List<KeyboardButton>>();
            var currentLine = new List<KeyboardButton>();
            
            foreach(var line in coreKeyboard.Buttons)
            {
                foreach(var button in line)
                {
                    currentLine.Add(new KeyboardButton(button.Title));
                }
                tgButtonsList.Add(currentLine);
                currentLine = new List<KeyboardButton>();
            }

            keyboard.Keyboard = tgButtonsList;

            return keyboard;
        }
    }
}