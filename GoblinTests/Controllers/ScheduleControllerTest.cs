using Goblin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Controllers
{
    [TestClass]
    public class ScheduleControllerTest
    {
        private ScheduleController control;

        [TestInitialize]
        public void Init()
        {
            control = new ScheduleController();
        }

        [TestMethod]
        public void IndexViewResultNotNull()
        {
            var result = control.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async void ShowViewResultNotNull()
        {
            var result = await control.Show(-1) as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}