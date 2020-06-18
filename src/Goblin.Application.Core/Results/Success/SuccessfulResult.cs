using Goblin.Application.Core.Abstractions;

namespace Goblin.Application.Core.Results.Success
{
    public class SuccessfulResult : IResult
    {
        public bool IsSuccessful => true;

        public string Message { get; set; }

        // public MediaAttachment[] Attachments { get; set; } //TODO:
        public object Keyboard { get; set; } //TODO:
    }
}