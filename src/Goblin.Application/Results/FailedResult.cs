using System.Collections.Generic;
using System.Linq;
using Goblin.Application.Abstractions;

namespace Goblin.Application.Results
{
    public class FailedResult : IResult
    {
        public bool IsSuccessful => false;
        
        public List<string> Errors { get; }

        public FailedResult(List<string> errors)
        {
            Errors = errors;
        }

        public override string ToString()
        {
            return string.Join('\n', Errors);
        }
    }
}