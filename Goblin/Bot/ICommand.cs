using System.Collections.Generic;

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
        string Result { get; set; }

        void Execute(string param);
    }
}