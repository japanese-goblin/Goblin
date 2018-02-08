using System;
using System.Collections.Generic;

namespace Goblin
{
    public partial class Persons
    {
        public int Id { get; set; }
        public string City { get; set; }
        public int CityId { get; set; }
        public short GroupId { get; set; }
        public bool Schedule { get; set; }
        public bool Weather { get; set; }
        public int VkId { get; set; }
    }
}
