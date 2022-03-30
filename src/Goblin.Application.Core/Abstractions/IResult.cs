using Goblin.Application.Core.Models;

namespace Goblin.Application.Core.Abstractions;

public interface IResult
{
    bool IsSuccessful { get; }

    public string Message { get; set; }
    public CoreKeyboard Keyboard { get; set; }
}