using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class MapCreateDto : MapEditDto
    {
        [Required]
        public string Id { get; set; } = null!;
    }
}
