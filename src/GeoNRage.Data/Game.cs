using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GeoNRage.Data
{
    [SuppressMessage("Usage", "CA2227", Justification = "Database entity")]
    public class Game : GameBase
    {
        public Game()
        {
            Players = new HashSet<string>();
            Maps = new HashSet<string>();
            Values = new HashSet<Value>();
        }

        public DateTime CreationDate { get; set; }

        public int Rounds { get; set; }

        public ICollection<string> Players { get; set; }

        public ICollection<string> Maps { get; set; }

        public ICollection<Value> Values { get; set; }

        public int this[string key]
        {
            get => Values.FirstOrDefault(x => x.Key == key)?.Score ?? 0;
            set
            {
                Value? v = Values.FirstOrDefault(x => x.Key == key);
                if (v is null)
                {
                    Values.Add(new Value { Score = value, Key = key, GameId = Id });
                }
                else
                {
                    Values.First(x => x.Key == key).Score = value;
                }
            }
        }
    }
}
