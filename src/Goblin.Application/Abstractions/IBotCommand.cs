using System.Threading.Tasks;
using VkNet.Model;

namespace Goblin.Application.Abstractions
{
    public interface IBotCommand
    {
        string Name { get; }
        string Description { get; }
        string Usage { get; }
        bool IsAdminCommand { get; }

        string[] Aliases { get; }

        Task<IResult> Execute(Message msg);
        Task<IResult> CanExecute(Message msg);
    }
}