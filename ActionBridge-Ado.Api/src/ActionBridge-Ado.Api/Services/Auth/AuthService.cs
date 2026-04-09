using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace ActionBridge_Ado.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IConfiguration config, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var clientId = _config["AzureAd:ClientId"];
        var clientSecret = _config["AzureAd:ClientSecret"];
        var tenantId = _config["AzureAd:TenantId"];
        var redirectUri = _config["Auth:RedirectUri"];

        // Get the user's token from the request
        var userToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(userToken))
        {
            _logger.LogWarning("No user token provided");
            throw new UnauthorizedAccessException("No user token provided");
        }

        var app = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .WithRedirectUri(redirectUri)
            .Build();

        var scopes = new[] { "https://app.vssps.visualstudio.com/.default" };

        try
        {
            var result = await app.AcquireTokenOnBehalfOf(scopes, new UserAssertion(userToken))
                .ExecuteAsync();

            _logger.LogInformation("Successfully received access token");
            return result.AccessToken;
        }
        catch (MsalException ex)
        {
            _logger.LogError(ex, "Token exchange failed");
            throw;
        }
    }
}
