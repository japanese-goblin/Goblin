using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;

namespace Goblin.Application.Core.Results.Success;

public class SuccessfulResult : IResult
{
    public bool IsSuccessful => true;

    public string Message { get; set; }

    // public MediaAttachment[] Attachments { get; set; } //TODO:
    public CoreKeyboard Keyboard { get; set; }

    public SuccessfulResult(string message = "")
    {
        Message = message;
    }
}