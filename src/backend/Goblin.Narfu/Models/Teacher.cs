using System.Text.Json.Serialization;

namespace Goblin.Narfu.Models;

public class Teacher
{
    [JsonPropertyName("lecturerOid")]
    public int Id { get; set; }

    [JsonPropertyName("fio")]
    public string Name { get; set; }

    [JsonPropertyName("chair")]
    public string Depart { get; set; }
}