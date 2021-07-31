using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Maps
{
    public record MapDto(string Id, string Name, bool IsMapForGame);

    public class MapEditDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public bool IsMapForGame { get; set; }
    }
}
