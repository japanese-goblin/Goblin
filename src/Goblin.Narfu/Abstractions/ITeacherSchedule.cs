using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;

namespace Goblin.Narfu.Abstractions
{
    public interface ITeacherSchedule
    {
        public Task<IEnumerable<Lesson>> GetSchedule(int teacherId);

        public Task<TeacherLessonsViewModel> GetLimitedSchedule(int teacherId, int limit = 10);

        public Task<Teacher[]> FindByName(string name);
    }
}