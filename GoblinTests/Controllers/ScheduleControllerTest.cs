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
        public void ShowViewResultNotNull()
        {
            var result = control.Show(-1) as ViewResult;
            Assert.IsNotNull(result);
        }

        //[TestMethod]
        //public void GetScheduleLessonsCount()
        //{
        //    var result = control.GetSchedule(6929);
        //    Assert.IsTrue(result.Count > 0, "result.Count > 0");
        //}
    }
}