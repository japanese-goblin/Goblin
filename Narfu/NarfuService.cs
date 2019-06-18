using Microsoft.Extensions.Logging;
using Narfu.Schedule;

namespace Narfu
{
    public class NarfuService
    {
        public NarfuService(ILogger logger)
        {
            Students = new StudentsSchedule(logger);
            Teachers = new TeachersSchedule(logger);
        }

        #region categories
        public StudentsSchedule Students;
        public TeachersSchedule Teachers;
        #endregion
    }
}