using System;
using System.Collections.Generic;

namespace GeoNRage.Server.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Challenge> Challenges { get; set; } = new HashSet<Challenge>();
    }
}
