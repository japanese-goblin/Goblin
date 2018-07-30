using Goblin.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void IndexViewResultNotNull()
        {
            HomeController controller = new HomeController();

            var result = controller.Index();

            Assert.IsNotNull(result);
        }
    }
}