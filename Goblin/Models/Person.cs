namespace Goblin.Models
{
    public class Person
    {
        public int ID { get; set; }
        public int VkID { get; set; }
        public short GroupID { get; set; }
        public bool Schedule { get; set; }
        public bool Weather { get; set; }
        public string City { get; set; }
        public int CityID { get; set; }
    }
}