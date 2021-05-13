using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Shared.Dtos
{
    public class GameDto : GameLightDto
    {
        public ICollection<PlayerDto> Players { get; set; } = new HashSet<PlayerDto>();

        public ICollection<GameMapDto> GameMaps { get; set; } = new HashSet<GameMapDto>();

        public ICollection<ValueDto> Values { get; set; } = new HashSet<ValueDto>();

        public int this[int mapId, int playerId, int round]
        {
            get => Values.FirstOrDefault(x => x.MapId == mapId && x.PlayerId == playerId && x.Round == round)?.Score ?? 0;
            set
            {
                ValueDto? v = Values.FirstOrDefault(x => x.MapId == mapId && x.PlayerId == playerId && x.Round == round);
                if (v is null)
                {
                    Values.Add(new ValueDto
                    {
                        Score = value,
                        MapId = mapId,
                        PlayerId = playerId,
                        Round = round,
                        GameId = Id
                    });
                }
                else
                {
                    Values.First(x => x.MapId == mapId && x.PlayerId == playerId && x.Round == round).Score = value;
                }
            }
        }
    }
}
