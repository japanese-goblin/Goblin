using System.Threading.Tasks;
using Vk.Models.Messages;

namespace Goblin.Bot
{
    public interface ICommand
    {
        string Name { get; }
        string Decription { get; }
        string Usage { get; }
        string[] Allias { get; }
        Category Category { get; }
        bool IsAdmin { get; }

        Task<CommandResponse> Execute(Message msg);
        (bool Success, string Text) CanExecute(Message msg);
    }
}
