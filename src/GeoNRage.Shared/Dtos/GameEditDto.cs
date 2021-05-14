using System;
using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class GameEditDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }
    }
}
