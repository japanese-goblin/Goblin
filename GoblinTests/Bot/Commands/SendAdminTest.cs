using Goblin.Bot.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Bot.Commands
{
    [TestClass]
    public class SendAdminTest
    {
        [TestMethod]
        public void CanExecuteWithEmptyParam()
        {
            var c = new SendAdminCommand();
            Assert.AreEqual(false, c.CanExecute(""));
        }
    }
}