using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Data.Entities
{
    public class Game
    {
        public Game()
        {
            Players = new HashSet<Player>();
            Maps = new HashSet<Map>();
            Values = new HashSet<Value>();
        }

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Locked { get; set; }

        public DateTime CreationDate { get; set; }

        public int Rounds { get; set; }

        public ICollection<Player> Players { get; set; }

        public ICollection<Map> Maps { get; set; }

        public ICollection<Value> Values { get; set; }

        public int this[int mapId, int playerId, int round]
        {
            get => Values.FirstOrDefault(x => x.MapId == mapId && x.PlayerId == playerId && x.Round == round)?.Score ?? 0;
            set
            {
                Value? v = Values.FirstOrDefault(x => x.MapId == mapId && x.PlayerId == playerId && x.Round == round);
                if (v is null)
                {
                    Values.Add(new Value
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
