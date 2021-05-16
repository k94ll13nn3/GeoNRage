using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class ChallengeImportDto
    {
        [Required]
        [Url]
        public string Link { get; set; } = null!;

        [Required]
        public bool PersistData { get; set; }
    }
}
