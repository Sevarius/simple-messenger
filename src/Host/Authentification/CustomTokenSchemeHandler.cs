using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Host.Authentification;

internal sealed class CustomTokenSchemeHandler : AuthenticationHandler<BearerTokenOptions>
{
    public CustomTokenSchemeHandler(
        IOptionsMonitor<BearerTokenOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) {}

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Sid, this.GetTokenFromQuery())
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, this.Scheme.Name));
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private string GetTokenFromQuery()
        => this.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
}

