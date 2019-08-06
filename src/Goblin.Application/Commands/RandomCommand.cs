using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using VkNet.Model;

namespace Goblin.Application.Commands
{
    public class RandomCommand : IBotCommand
    {
        public string Name => "рандом";
        public string Description => "test";
        public string Usage => "рандом";
        
        public bool IsAdminCommand => false;
        
        public string[] Aliases => new[] { "рандом" };
        
        public Task<IResult> Execute(Message msg)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> CanExecute(Message msg)
        {
            throw new System.NotImplementedException();
        }
    }
}