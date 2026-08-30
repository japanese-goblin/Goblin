using Goblin.Narfu.Models;

namespace Goblin.Narfu.ViewModels;

public record LessonsViewModel(IReadOnlyCollection<Lesson> Lessons, DateTime Date);