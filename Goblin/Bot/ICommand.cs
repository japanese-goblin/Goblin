using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Models.Keyboard;

namespace Goblin.Bot
{
    public interface ICommand
    {
        string Name { get; }
        string Decription { get; }
        string Usage { get; }
        List<string> Allias { get; }
        Category Category { get; }
        bool IsAdmin { get; }
        string Message { get; set; }
        Keyboard Keyboard { get; set; }

        Task Execute(string param, int id = 0);
        bool CanExecute(string param, int id = 0);
    }
}