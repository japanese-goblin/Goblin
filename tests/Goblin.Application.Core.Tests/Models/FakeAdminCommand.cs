using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core.Tests.Models
{
    public class FakeAdminCommand : ITextCommand
    {
        public bool IsAdminCommand => true;
        public string[] Aliases => new[] { "demo" };

        public Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "demo"
            });
        }
    }
}