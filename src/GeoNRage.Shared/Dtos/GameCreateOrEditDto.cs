using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
    public class GameCreateOrEditDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        [Required]
        public ICollection<int> PlayerIds { get; set; } = new HashSet<int>();

        [Required]
        public ICollection<GameMapCreateOrEditDto> Maps { get; set; } = new HashSet<GameMapCreateOrEditDto>();
    }
}
