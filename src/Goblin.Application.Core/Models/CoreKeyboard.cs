﻿using System.Collections.Generic;
using System.Linq;

namespace Goblin.Application.Core.Models
{
    public class CoreKeyboard
    {
        public bool IsOneTime { get; set; }
        public bool IsInline { get; set; }
        public List<List<CoreKeyboardButton>> Buttons { get; set; }

        private List<CoreKeyboardButton> LastLine => Buttons.Last();

        public CoreKeyboard(bool isOneTime = true)
        {
            Buttons = new List<List<CoreKeyboardButton>>
            {
                new List<CoreKeyboardButton>()
            };
            IsOneTime = isOneTime;
        }

        public CoreKeyboard AddButton(string text, CoreKeyboardButtonColor color, string payloadKey, string payloadValue)
        {
            var button = new CoreKeyboardButton
            {
                Title = text,
                Color = color
            };
            button.SetPayload(payloadKey, payloadValue);
            LastLine.Add(button);

            return this;
        }

        public CoreKeyboard AddLine()
        {
            Buttons.Add(new List<CoreKeyboardButton>());

            return this;
        }

        public CoreKeyboard AddReturnToMenuButton(bool addNewLine = true)
        {
            if(IsInline)
            {
                return this;
            }

            if(addNewLine)
            {
                AddLine();
            }

            var button = new CoreKeyboardButton
            {
                Title = "Вернуться в главное меню",
                Color = CoreKeyboardButtonColor.Default
            };
            button.SetPayload("command", "start");
            LastLine.Add(button);

            return this;
        }
    }
}