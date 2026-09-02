using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels;

public record ExamsViewModel(IReadOnlyCollection<Lesson> Lessons) : LessonsViewModel(Lessons, DateTime.Now.Date);
