using System.Collections.Generic;

namespace GeoNRage.Server.Entities
{
    public class Player
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public ICollection<PlayerScore> PlayerScores { get; set; } = new HashSet<PlayerScore>();
    }
}
