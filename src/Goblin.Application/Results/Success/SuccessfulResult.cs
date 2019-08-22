using Goblin.Application.Abstractions;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Results.Success
{
    public class SuccessfulResult : IResult
    {
        public bool IsSuccessful => true;

        public string Message { get; set; }
        public MediaAttachment[] Attachments { get; set; }
        public MessageKeyboard Keyboard { get; set; }
    }
}