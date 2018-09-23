﻿using Goblin.Bot.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Bot.Commands
{
    [TestClass]
    public class RandomTest
    {
        private FlipCommand _c;

        [TestInitialize]
        public void Init()
        {
            _c = new FlipCommand();
        }

        [TestMethod]
        public void CanExecuteWithEmptyParam()
        {
            var result = _c.CanExecute("");
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CanExecuteWithWrongParam()
        {
            var result = _c.CanExecute("ы");
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Execute()
        {
            _c.Execute("1 или 2");
            Assert.IsTrue(_c.Message == "1" || _c.Message == "2");
        }
    }
}