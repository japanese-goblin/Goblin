using System;

namespace Goblin.Narfu.Models;

public record Lesson
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

    public string Link { get; set; }

    public bool IsExam => LessonType == LessonType.Exam;

    public LessonType LessonType => GetLessonType();

    private LessonType GetLessonType()
    {
        if(Type.Contains("экзамен", StringComparison.InvariantCultureIgnoreCase) ||
           Type.Contains("зачет", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Exam;
        }

        if(Type.Contains("практическ", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Practical;
        }

        if(Type.Contains("лабораторн", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Laboratory;
        }
        
        if(Type.Contains("лекции", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Lecture;
        }
        
        if(Type.Contains("консультация", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Consultation;
        }

        return LessonType.Unknown;
    }
}