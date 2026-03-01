namespace ActionBridge_Ado.Api;

public interface IAuthService
{
    Task<string> GetAccessTokenAsync();
}
