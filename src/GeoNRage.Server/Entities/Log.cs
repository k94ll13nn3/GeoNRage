using System;

namespace GeoNRage.Server.Entities
{
    public class Log
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; } = null!;

        public string Level { get; set; } = null!;

        public string Exception { get; set; } = null!;

        public string Stacktrace { get; set; } = null!;

        public string Callsite { get; set; } = null!;

        public string Logger { get; set; } = null!;
    }
}
