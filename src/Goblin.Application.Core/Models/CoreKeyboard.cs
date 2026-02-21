namespace Goblin.Application.Core.Models;

public class CoreKeyboard(bool isOneTime = true)
{
    public const string ReturnToMainMenuText = "Вернуться в главное меню";

    public bool IsOneTime { get; set; } = isOneTime;
    public bool IsInline { get; set; }
    public List<List<CoreKeyboardButton>> Buttons { get; set; } = [[]];

    private List<CoreKeyboardButton> LastLine => Buttons.Last();

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
        Buttons.Add([]);

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
            Title = ReturnToMainMenuText,
            Color = CoreKeyboardButtonColor.Default
        };
        button.SetPayload("command", "start");
        LastLine.Add(button);

        return this;
    }

    public CoreKeyboard RemoveReturnToMenuButton()
    {
        var lineWithReturnButton = Buttons
                .SingleOrDefault(x =>
                                         x.Any(z => z.Title == ReturnToMainMenuText));

        var returnButton = lineWithReturnButton?.SingleOrDefault(x => x.Title == ReturnToMainMenuText);
        if(returnButton != null)
        {
            lineWithReturnButton.Remove(returnButton);
        }

        return this;
    }
}