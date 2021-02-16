using System;
using System.Collections.Generic;

namespace GeoNRage.Server.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Locked { get; set; }

        public DateTime CreationDate { get; set; }

        public int Rounds { get; set; }

        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

        public ICollection<Map> Maps { get; set; } = new HashSet<Map>();

        public ICollection<Value> Values { get; set; } = new HashSet<Value>();
    }
}
