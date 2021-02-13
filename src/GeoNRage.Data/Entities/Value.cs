using System;

namespace GeoNRage.Data.Entities
{
    public class Value
    {
        public int GameId { get; set; }

        public Game Game { get; set; } = null!;

        public int PlayerId { get; set; }

        public Player Player { get; set; } = null!;

        public int MapId { get; set; }

        public Map Map { get; set; } = null!;

        public int Round { get; set; }

        public int Score { get; set; }
    }
}
