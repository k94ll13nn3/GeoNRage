using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class PlayerCreateDto : PlayerEditDto
    {
        [Required]
        public string Id { get; set; } = null!;
    }
}
