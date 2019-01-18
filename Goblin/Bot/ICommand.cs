using System.Threading.Tasks;
using Vk.Models.Keyboard;
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
        string Message { get; set; }
        Keyboard Keyboard { get; set; }

        Task Execute(Message msg);
        bool CanExecute(Message msg);
    }
}