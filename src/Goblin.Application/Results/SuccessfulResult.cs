using Goblin.Application.Abstractions;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Results
{
    public class SuccessfulResult : IResult
    {
        public bool IsSuccessful => true;

        public string Message { get; }
        public MediaAttachment[] Attachments { get; }
        public MessageKeyboard Keyboard { get; } //TODO: change type
    }
}