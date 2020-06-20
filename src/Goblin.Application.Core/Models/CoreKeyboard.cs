using System.Collections.Generic;

namespace Goblin.Application.Core.Models
{
    public class CoreKeyboard
    {
        public bool IsOneTime { get; set; }
        public bool IsInline { get; set; } //TODO:
        public List<List<CoreKeyboardButton>> Buttons { get; set; }
        private List<CoreKeyboardButton> _currentLine;

        public CoreKeyboard(bool isOneTime = true)
        {
            Buttons = new List<List<CoreKeyboardButton>>();
            _currentLine = new List<CoreKeyboardButton>();
            IsOneTime = isOneTime;
        }

        public CoreKeyboard AddButton(string text, CoreKeyboardButtonColor color, string payloadKey, string payloadValue)
        {
            var button = new CoreKeyboardButton
            {
                Title = text,
                Color = color,
            };
            button.SetPayload(payloadKey, payloadValue);
            _currentLine.Add(button);

            return this;
        }

        public CoreKeyboard AddLine()
        {
            Buttons.Add(_currentLine);
            _currentLine = new List<CoreKeyboardButton>();

            return this;
        }

        public CoreKeyboard AddReturnToMenuButton(bool addNewLine = true)
        {
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
            _currentLine.Add(button);

            return this;
        }
    }
}