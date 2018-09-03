using System;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Helpers
{
    [TestClass]
    public class ScheduleHelperTest
    {
        private const int groupId = 351617;

        [TestMethod]
        public async Task GetSchedule_Right_Schedule()
        {
            var sched = await ScheduleHelper.GetScheduleAtDate(new DateTime(2018, 9, 12), groupId);
            Assert.IsTrue(!sched.Contains("отсутствует"));
        }

        [TestMethod]
        public async Task GetSchedule_August_Empty()
        {
            var sched = await ScheduleHelper.GetScheduleAtDate(new DateTime(2018, 8, 12), groupId);
            Assert.IsTrue(sched.Contains("отсутствует"));
        }

        [TestMethod]
        public async Task GetSchedule_Right_List()
        {
            var sched = await ScheduleHelper.GetSchedule(groupId);
            Assert.IsFalse(sched.IsError);
        }
    }
}