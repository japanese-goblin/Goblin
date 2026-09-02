namespace Goblin.Narfu.Models;

/// <summary>
///     Модель занятия
/// </summary>
public class Lesson : IEquatable<Lesson>
{
    /// <summary>
    ///     Идентификатор
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Тип
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Дата начала
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    ///     Дата окончания
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    ///     Номер (внутри одного дня)
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    ///     Адрес, где будет проходить занятие
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    ///     Номер аудитории, где будет проходить занятие
    /// </summary>
    public string Auditory { get; set; }

    /// <summary>
    ///     Преподаватель
    /// </summary>
    public string Teacher { get; set; }

    /// <summary>
    ///     Список групп
    /// </summary>
    public string Groups { get; set; }

    /// <summary>
    ///     Ссылка на курс (если есть)
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    ///     Форматированное время начала и окончания
    /// </summary>
    public string StartEndTime => $"{StartTime:HH:mm} - {EndTime:HH:mm}";

    /// <summary>
    ///     Тип занятия
    /// </summary>
    public LessonType LessonType => GetLessonType();

    /// <summary>
    ///     Является ли занятие экзаменом
    /// </summary>
    public bool IsExam => LessonType == LessonType.Exam;

    private LessonType GetLessonType()
    {
        if (string.IsNullOrWhiteSpace(Type))
        {
            return LessonType.Unknown;
        }

        if (Type.Contains("экзамен", StringComparison.InvariantCultureIgnoreCase) ||
            Type.Contains("зачет", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Exam;
        }

        if (Type.Contains("практическ", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Practical;
        }

        if (Type.Contains("лабораторн", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Laboratory;
        }

        if (Type.Contains("лекция", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Lecture;
        }

        if (Type.Contains("консультация", StringComparison.InvariantCultureIgnoreCase))
        {
            return LessonType.Consultation;
        }

        return LessonType.Unknown;
    }

    public bool Equals(Lesson? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Type == other.Type &&
               Name == other.Name &&
               Number == other.Number &&
               Auditory == other.Auditory &&
               Teacher == other.Teacher &&
               Groups == other.Groups;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((Lesson)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Name, Number, Auditory, Teacher, Groups);
    }
}
