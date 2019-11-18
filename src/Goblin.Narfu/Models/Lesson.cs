using System;

namespace Goblin.Narfu.Models
{
    public class Lesson : IEquatable<Lesson>
    {
        public string Id { get; set; }
        
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string StartEndTime { get; set; }
        public int Number { get; set; }
        public string Address { get; set; }
        public string Auditory { get; set; }
        public string Teacher { get; set; }
        public string Groups { get; set; }

        public bool Equals(Lesson other)
        {
            return Id == other?.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool IsExam()
        {
            return Type.Contains("экзамен", StringComparison.OrdinalIgnoreCase) ||
                   Type.Contains("зачет", StringComparison.OrdinalIgnoreCase);
        }
    }
}