using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Data.Dtos
{
    public class MapCreateOrEditDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
