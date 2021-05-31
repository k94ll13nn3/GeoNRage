using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeImportDto
    {
        [Required]
        public string GeoGuessrId { get; set; } = null!;

        [Required]
        public bool PersistData { get; set; }

        [Required]
        public bool OverrideData { get; set; }
    }
}
