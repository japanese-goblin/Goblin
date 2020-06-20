using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;

namespace Goblin.Application.Core.Results.Failed
{
    public class FailedResult : IResult
    {
        public bool IsSuccessful => false;

        public string Message { get; set; }
        public CoreKeyboard Keyboard { get; set; }

        protected FailedResult()
        {
        }

        public FailedResult(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}