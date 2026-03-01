using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace ActionBridge_Ado.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var clientId = _config["AzureAd:ClientId"];
        var clientSecret = _config["AzureAd:ClientSecret"];
        var tenantId = _config["AzureAd:TenantId"];

        // Get the user's token from the request
        var userToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(userToken))
        {
            throw new UnauthorizedAccessException("No user token provided");
        }

        var app = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .WithRedirectUri("http://localhost")
            .Build();

        var scopes = new[] { "https://app.vssps.visualstudio.com/.default" };

        try
        {
            var result = await app.AcquireTokenOnBehalfOf(scopes, new UserAssertion(userToken))
                .ExecuteAsync();

            return result.AccessToken;
        }
        catch (MsalException ex)
        {
            Console.WriteLine($"Token exchange failed: {ex.Message}");
            throw;
        }
    }
}
