using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoNRage.Data
{
    public class Game : GameBase
    {
        public Game()
        {
            Columns = new HashSet<string>();
            Rows = new HashSet<string>();
            Maps = new HashSet<string>();
            Values = new HashSet<Value>();
        }

        public DateTime CreationDate { get; set; }

        public bool Locked { get; set; }

        public ICollection<string> Columns { get; set; }

        public ICollection<string> Rows { get; set; }

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
