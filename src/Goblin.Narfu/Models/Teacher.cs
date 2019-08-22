using Newtonsoft.Json;

namespace Goblin.Narfu.Models
{
    public class Teacher
    {
        [JsonProperty("lecturerOid")]
        public int Id { get; set; }

        [JsonProperty("fio")]
        public string Name { get; set; }

        [JsonProperty("chair")]
        public string Depart { get; set; }
    }
}