using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class GameMapCreateOrEditDto
    {
        [Required]
        public int MapId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Link { get; set; }
    }
}
