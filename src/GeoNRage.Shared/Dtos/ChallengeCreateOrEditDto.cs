using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeCreateOrEditDto
    {
        public int Id { get; set; }

        [Required]
        public string MapId { get; set; } = null!;

        [Required]
        public string GeoGuessrId { get; set; } = null!;
    }
}
