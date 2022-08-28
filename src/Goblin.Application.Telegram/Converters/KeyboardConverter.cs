﻿using System.Collections.Generic;
using Goblin.Application.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Goblin.Application.Telegram.Converters;

public static class KeyboardConverter
{
    public static IReplyMarkup FromCoreToTg(CoreKeyboard coreKeyboard)
    {
        if(coreKeyboard is null)
        {
            return null;
        }
        
        coreKeyboard.RemoveReturnToMenuButton();

        return coreKeyboard.IsInline ? GenerateInlineKeyboard() : GenerateReplyKeyboard();

        IReplyMarkup GenerateReplyKeyboard()
        {
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
                
            var keyboard = new ReplyKeyboardMarkup(tgButtonsList)
            {
                OneTimeKeyboard = coreKeyboard.IsOneTime,
                ResizeKeyboard = true
            };

            return keyboard;
        }

        IReplyMarkup GenerateInlineKeyboard()
        {
            var tgButtonsList = new List<List<InlineKeyboardButton>>();
            var currentLine = new List<InlineKeyboardButton>();

            foreach(var line in coreKeyboard.Buttons)
            {
                foreach(var button in line)
                {
                    currentLine.Add(InlineKeyboardButton.WithCallbackData(button.Title, button.Payload));
                }

                tgButtonsList.Add(currentLine);
                currentLine = new List<InlineKeyboardButton>();
            }

            var keyboard = new InlineKeyboardMarkup(tgButtonsList);

            return keyboard;
        }
    }
}