using System.Collections.Generic;

namespace GeoNRage.Server.Entities
{
    public class Map
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();

        public ICollection<Value> Values { get; set; } = new HashSet<Value>();
    }
}
