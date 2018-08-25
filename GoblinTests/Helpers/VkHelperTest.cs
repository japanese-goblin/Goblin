using Goblin.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Helpers
{
    [TestClass]
    public class VkHelperTest
    {
        [TestMethod]
        public void SendMessage_Admins_True()
        {
            var res = VkHelper.SendMessage(VkHelper.DevelopersID, "Hello from tests");
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SendMessage_Durov_False()
        {
            var res = VkHelper.SendMessage(1, "Hello from tests");
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void GetUserName_Me_MyName()
        {
            var res = VkHelper.GetUserName(***REMOVED***);
            Assert.AreEqual("Кирилл Кузнецов", res);
        }

        [TestMethod]
        public void GetUserName_Unknown_Empty()
        {
            var res = VkHelper.GetUserName(0);
            Assert.AreEqual("", res);
        }
    }
}