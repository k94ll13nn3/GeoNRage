using System;
using System.Collections.Generic;

namespace GeoNRage.Shared.Dtos
{
    public class GameCreateOrEditDto
    {
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public ICollection<int> PlayerIds { get; set; } = new HashSet<int>();

        public ICollection<GameMapCreateOrEditDto> Maps { get; set; } = new HashSet<GameMapCreateOrEditDto>();
    }
}
