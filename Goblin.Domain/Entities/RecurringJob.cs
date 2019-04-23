namespace Goblin.Domain.Entities
{
    public class RecurringJob
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public int Conversation { get; set; }
        public int NarfuGroup { get; set; }
        public string WeatherCity { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }
}