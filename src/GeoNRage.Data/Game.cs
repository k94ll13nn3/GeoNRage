using System.Collections.Generic;

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

        public ICollection<string> Columns { get; set; }

        public ICollection<string> Rows { get; set; }

        public ICollection<string> Maps { get; set; }

        public ICollection<Value> Values { get; set; }
    }
}
