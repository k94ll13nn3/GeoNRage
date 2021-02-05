using System;

namespace GeoNRage.Data
{
    public class Value
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int GameId { get; set; }

        public Game Game { get; set; } = null!;

        public string Key { get; set; } = string.Empty;

        public int Score { get; set; }

        public string GetMap() => Key.Split('_')[0];

        public string GetPlayer() => Key.Split('_')[1];
    }
}
