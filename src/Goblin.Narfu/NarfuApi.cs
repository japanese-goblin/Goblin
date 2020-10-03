using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Schedule;

namespace Goblin.Narfu
{
    public class NarfuApi : INarfuApi
    {
        public ITeacherSchedule Teachers { get; }
        public IStudentsSchedule Students { get; }

        public NarfuApi(string groupsLink)
        {
            Teachers = new TeachersSchedule();
            Students = new StudentsSchedule(groupsLink);
        }
    }
}