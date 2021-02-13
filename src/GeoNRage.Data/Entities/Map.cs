using System.Collections.Generic;

namespace GeoNRage.Data.Entities
{
    public class Map
    {
        public Map()
        {
            Games = new HashSet<Game>();
            Values = new HashSet<Value>();
        }

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Game> Games { get; set; }

        public ICollection<Value> Values { get; set; }
    }
}
