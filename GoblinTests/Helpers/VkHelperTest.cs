using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Helpers
{
    [TestClass]
    public class VkHelperTest
    {
        [TestMethod]
        public async Task SendMessage_Admins_True()
        {
            var res = await VkHelper.SendMessage(VkHelper.DevelopersID, "Hello from tests");
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public async Task SendMessage_Durov_False()
        {
            var res = await VkHelper.SendMessage(1, "Hello from tests");
            Assert.AreEqual(false, res);
        }

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