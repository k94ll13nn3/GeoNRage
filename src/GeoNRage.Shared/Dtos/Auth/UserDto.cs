using System.Collections.Generic;

namespace GeoNRage.Shared.Dtos.Auth
{
    public class UserDto
    {
        public bool IsAuthenticated { get; set; }

        public string UserName { get; set; } = string.Empty;

        public Dictionary<string, string> Claims { get; set; } = new();
    }
}
