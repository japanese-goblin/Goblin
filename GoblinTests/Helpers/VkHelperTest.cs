using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Helpers
{
    [TestClass]
    public class VkHelperTest
    {
        [TestMethod]
        public async Task GetUserName_Me_MyName()
        {
            var res = await VkHelper.GetUserName(***REMOVED***);
            Assert.AreEqual("Кирилл Кузнецов", res);
        }

        [TestMethod]
        public async Task GetUserName_Unknown_Empty()
        {
            var res = await VkHelper.GetUserName(0);
            Assert.AreEqual("", res);
        }
    }
}