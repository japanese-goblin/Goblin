using System.Collections.Generic;
using System.Linq;
using Goblin.Application.Abstractions;

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

        public override string ToString()
        {
            return string.Join('\n', Errors.Select(x => $"{x.Key} - {x.Value}"));
        }
    }
}