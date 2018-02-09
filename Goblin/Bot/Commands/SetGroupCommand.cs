using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goblin.Bot.Commands
{
    public class SetGroupCommand : ICommand
    {
        public string Name => "устгр";
        public string Decription => "Установить группу для получения расписания";
        public string Usage => "устгр 3124";
        public List<string> Allias => new List<string>() { "устгр" };
        public Category Category => Category.SAFU;
        public bool IsAdmin => false;
        public string Result { get; set; }

        public void Execute(string param)
        {
            throw new NotImplementedException();
        }
    }
}
