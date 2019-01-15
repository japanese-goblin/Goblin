using System.Threading.Tasks;
using Vk.Models.Keyboard;

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

        Task Execute(string param, long id = 0);
        bool CanExecute(string param, long id = 0);
    }
}