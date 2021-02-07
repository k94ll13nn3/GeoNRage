using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Security
{
    public class GeoNRageAuthenticationHandler : AuthenticationHandler<GeoNRageAuthenticationOptions>
    {
        public const string GeoNRageAuthenticationScheme = "GeoNRageAuthenticationScheme";

        public GeoNRageAuthenticationHandler(IOptionsMonitor<GeoNRageAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string host = Request.Headers["Host"];
            if (host == Configuration.GetValue<string>("AllowedAuthority"))
            {
                Claim[] claims = new[] { new Claim("GeoNRage", "Ok") };
                var identity = new ClaimsIdentity(claims, nameof(GeoNRageAuthenticationHandler));
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail($"Authority '{host}' ({Request.Headers["Referer"]}) / ({Request.Headers[":authority"]}): not allowed."));
        }
    }
}
