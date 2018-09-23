using Goblin.Bot.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Bot.Commands
{
    [TestClass]
    public class DebugTest
    {
        [TestMethod]
        public void Execute()
        {
            var c = new DebugCommand();
            c.Execute("");
            Assert.IsTrue(c.Message.Contains("Текущее время на сервере"));
        }
    }
}