namespace Goblin.Narfu.Abstractions
{
    public interface INarfuApi
    {
        public ITeacherSchedule Teachers { get; }
        public IStudentsSchedule Students { get; }
    }
}