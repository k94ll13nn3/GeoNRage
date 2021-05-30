using System;
using System.Collections.Generic;

namespace GeoNRage.Server.Entities
{
    public class Challenge
    {
        public int Id { get; set; }

        public string MapId { get; set; } = null!;

        public Map Map { get; set; } = null!;

        public int GameId { get; set; }

        public Game Game { get; set; } = null!;

        public Uri Link { get; set; } = null!;

        public ICollection<PlayerScore> PlayerScores { get; set; } = new HashSet<PlayerScore>();
    }
}
