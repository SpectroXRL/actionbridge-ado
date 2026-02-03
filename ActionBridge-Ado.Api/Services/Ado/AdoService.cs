
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;

namespace ActionBridge_Ado.Api.Services.Ado;

public class AdoService : IAdoService
{
    private readonly IAuthService _authService;

    public AdoService(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<string> CreateWorkItemAsync(string organization, string project, WorkItemRequest workItem)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TeamProjectReference>> GetProjectsAsync(string organizationUrl)
    {
        var uri = new Uri(organizationUrl);

        var entraIdAccessToken = await _authService.GetAccessTokenAsync();
        var credentials = new VssOAuthAccessTokenCredential(entraIdAccessToken);

        using var connection = new VssConnection(uri, credentials);
        using var projectClient = connection.GetClient<ProjectHttpClient>();

        try
        {
            var projects = await projectClient.GetProjects();
            return projects;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving projects: {ex.Message}");
            throw;
        }
    }
}
