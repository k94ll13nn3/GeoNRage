using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeImportDto
    {
        [Required]
        public string Link { get; set; } = null!;

        [Required]
        public bool PersistData { get; set; }
    }
}
