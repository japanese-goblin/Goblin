using System.Text.Json.Serialization;

namespace Goblin.Narfu.Models;

/// <summary>
///     Модель преподавателя
/// </summary>
/// <param name="Id">Идентификатор</param>
/// <param name="Name">Имя</param>
/// <param name="Depart">Кафедра</param>
public record Teacher(
    [property: JsonPropertyName("lecturerOid")]
    int Id,
    [property: JsonPropertyName("fio")]
    string Name,
    [property: JsonPropertyName("chair")]
    string Depart);
