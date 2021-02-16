using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class MapCreateOrEditDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
