using System.Collections.Generic;

namespace GeoNRage.Server.Entities
{
    public class Map
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<GameMap> GameMaps { get; set; } = new HashSet<GameMap>();

        public ICollection<Value> Values { get; set; } = new HashSet<Value>();
    }
}
