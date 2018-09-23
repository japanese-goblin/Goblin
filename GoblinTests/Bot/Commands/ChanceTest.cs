using Goblin.Bot.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Bot.Commands
{
    [TestClass]
    public class ChanceTest
    {
        private ChanceCommand c;

        [TestInitialize]
        public void Init() => c = new ChanceCommand();

        [TestMethod]
        public void CanExecuteWithEmptyParam()
        {
            var result = c.CanExecute("");
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ExecuteWithParams()
        {
            c.Execute("ы");
            Assert.IsTrue(c.Message.Contains("Вероятность"));
        }
    }
}