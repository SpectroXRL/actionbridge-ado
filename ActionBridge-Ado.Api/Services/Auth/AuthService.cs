using Microsoft.Identity.Client;

namespace ActionBridge_Ado.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var clientId = _config["AzureAd:ClientId"];
        var tenantId = _config["AzureAd:TenantId"];

        var app = PublicClientApplicationBuilder
            .Create(clientId)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .WithRedirectUri("http://localhost")
            .Build();

        var scopes = new[] { "https://app.vssps.visualstudio.com/.default" };

        try
        {
            var result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalException ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
            throw;
        }
    }
}
