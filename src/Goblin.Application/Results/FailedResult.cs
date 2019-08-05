using System.Collections.Generic;

namespace Goblin.Application.Results
{
    public class FailedResult : IResult
    {
        public bool IsSuccessful => false;
        
        public Dictionary<string, string> Errors { get; }

        public FailedResult(Dictionary<string, string> errors)
        {
            Errors = errors;
        }
    }
}