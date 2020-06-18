using Goblin.Application.Core.Abstractions;

namespace Goblin.Application.Core.Results.Failed
{
    public class FailedResult : IResult
    {
        public bool IsSuccessful => false;

        public string Error { get; }

        protected FailedResult()
        {
        }

        public FailedResult(string error)
        {
            Error = error;
        }

        public override string ToString()
        {
            return Error;
        }
    }
}