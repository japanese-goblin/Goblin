using Goblin.Application.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Goblin.Application.Telegram.Converters;

public static class KeyboardConverter
{
    public static ReplyMarkup? FromCoreToTg(CoreKeyboard? coreKeyboard)
    {
        if(coreKeyboard is null)
        {
            return null;
        }

        coreKeyboard.RemoveReturnToMenuButton();

        return coreKeyboard.IsInline ? GenerateInlineKeyboard() : GenerateReplyKeyboard();

        ReplyMarkup GenerateReplyKeyboard()
        {
            var tgButtonsList = new List<List<KeyboardButton>>();
            var currentLine = new List<KeyboardButton>();

            foreach(var line in coreKeyboard.Buttons)
            {
                currentLine.AddRange(line.Select(button => new KeyboardButton(button.Title)));

                tgButtonsList.Add(currentLine);
                currentLine = [];
            }

            var keyboard = new ReplyKeyboardMarkup(tgButtonsList)
            {
                OneTimeKeyboard = coreKeyboard.IsOneTime,
                ResizeKeyboard = true
            };

            return keyboard;
        }

        InlineKeyboardMarkup GenerateInlineKeyboard()
        {
            var tgButtonsList = new List<List<InlineKeyboardButton>>();
            var currentLine = new List<InlineKeyboardButton>();

            foreach(var line in coreKeyboard.Buttons)
            {
                currentLine.AddRange(line.Select(button => InlineKeyboardButton.WithCallbackData(button.Title, button.Payload)));

                tgButtonsList.Add(currentLine);
                currentLine = [];
            }

            var keyboard = new InlineKeyboardMarkup(tgButtonsList);

            return keyboard;
        }
    }
}