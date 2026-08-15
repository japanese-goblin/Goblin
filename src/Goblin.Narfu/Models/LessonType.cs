namespace Goblin.Narfu.Models;

/// <summary>
///     Тип занятия
/// </summary>
public enum LessonType
{
    /// <summary>
    ///     Неизвестно
    /// </summary>
    Unknown,

    /// <summary>
    ///     Экзамен
    /// </summary>
    Exam,

    /// <summary>
    ///     Практическое занятие
    /// </summary>
    Practical,
    
    /// <summary>
    ///     Лабораторная работа
    /// </summary>
    Laboratory,
    
    /// <summary>
    ///     Лекция
    /// </summary>
    Lecture,
    
    /// <summary>
    ///     Консультация
    /// </summary>
    Consultation
}