using System;
using Goblin.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Helpers
{
    [TestClass]
    public class ScheduleHelperTest
    {
        private const int groupId = 9092;

        [TestMethod]
        public void GetSchedule_Right_Schedule()
        {
            var sched = ScheduleHelper.GetScheduleAtDate(new DateTime(2018, 9, 12), groupId);
            Assert.IsTrue(!sched.Contains("отсутствует"));
        }

        [TestMethod]
        public void GetSchedule_August_Empty()
        {
            var sched = ScheduleHelper.GetScheduleAtDate(new DateTime(2018, 8, 12), groupId);
            Assert.IsTrue(sched.Contains("отсутствует"));
        }

        [TestMethod]
        public void GetSchedule_Right_List()
        {
            var sched = ScheduleHelper.GetSchedule(groupId, out var lessons);
            Assert.IsTrue(sched);
        }
    }
}