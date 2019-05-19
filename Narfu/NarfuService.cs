using Narfu.Schedule;

namespace Narfu
{
    public class NarfuService
    {
        public NarfuService()
        {
            Students = new StudentsSchedule();
            Teachers = new TeachersSchedule();
        }

        #region categories
        public StudentsSchedule Students;
        public TeachersSchedule Teachers;
        #endregion
    }
}