using System.Collections.Generic;

namespace GeoNRage.Models
{
    public class Game : GameBase
    {
        public Game(int id, string name, IEnumerable<string> columns, IEnumerable<string> rows, IEnumerable<string> maps, IDictionary<string, int> values)
            : base(id, name)
        {
            Columns = columns;
            Rows = rows;
            Maps = maps;
            Values = values;
        }

        public IEnumerable<string> Columns { get; }

        public IEnumerable<string> Rows { get; }

        public IEnumerable<string> Maps { get; }

        public IDictionary<string, int> Values { get; }
    }
}
