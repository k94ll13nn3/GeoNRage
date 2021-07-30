using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class GameCreateOrEditDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        [Required]
        public ICollection<string> PlayerIds { get; set; } = new HashSet<string>();

        [Required]
        public ICollection<GameChallengeCreateOrEditDto> Challenges { get; set; } = new HashSet<GameChallengeCreateOrEditDto>();
    }
}
