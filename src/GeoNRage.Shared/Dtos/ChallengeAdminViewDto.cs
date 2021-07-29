using System;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeAdminViewDto
    {
        public int Id { get; set; }

        public string MapId { get; set; } = null!;

        public string MapName { get; set; } = null!;

        public string GeoGuessrId { get; set; } = null!;

        public int GameId { get; set; }

        public string GameName { get; set; } = null!;

        public DateTime LastUpdate { get; set; }

        public bool UpToDate { get; set; }
    }
}
