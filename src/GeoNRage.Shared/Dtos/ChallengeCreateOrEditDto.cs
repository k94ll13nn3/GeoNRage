using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeCreateOrEditDto
    {
        [Required]
        public string MapId { get; set; } = null!;

        public string? Link { get; set; }
    }
}
