using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;

namespace Goblin.Narfu.Abstractions;

public interface IStudentsSchedule
{
    public Task<IEnumerable<Lesson>> GetSchedule(int realGroupId, DateTime? date = default);

    public Task<LessonsViewModel> GetScheduleAtDate(int realGroupId, DateTime date);

    public Task<ExamsViewModel> GetExams(int realGroupId);

    public bool IsCorrectGroup(int realGroupId);

    public Group GetGroupByRealId(int realGroupId);

    public string GenerateScheduleLink(int realGroupId, bool isWebCal = false);
}