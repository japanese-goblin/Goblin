using Goblin.Narfu.Schedule;

namespace Goblin.Narfu
{
    public class NarfuApi
    {
        public TeachersSchedule Teachers { get; }
        public StudentsSchedule Students { get; }

        public NarfuApi()
        {
            Teachers = new TeachersSchedule();
            Students = new StudentsSchedule();
        }
    }
}