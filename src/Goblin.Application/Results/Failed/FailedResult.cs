using Goblin.Application.Abstractions;

namespace Goblin.Application.Results.Failed
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