using Goblin.Application.Abstractions;

namespace Goblin.Application.Results
{
    public class FailedResult : IResult
    {
        public bool IsSuccessful => false;

        public string Error { get; }

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