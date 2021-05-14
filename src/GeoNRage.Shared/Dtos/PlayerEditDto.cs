using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerEditDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
