using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class MapEditDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public bool IsMapForGame { get; set; }
    }
}
