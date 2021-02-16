using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerCreateOrEditDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
