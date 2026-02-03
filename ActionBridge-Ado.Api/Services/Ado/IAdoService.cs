using Microsoft.TeamFoundation.Core.WebApi;

namespace ActionBridge_Ado.Api.Services.Ado;

public interface IAdoService
{
    Task<string> CreateWorkItemAsync(string organization, string project, WorkItemRequest workItem);
    Task<IEnumerable<TeamProjectReference>> GetProjectsAsync(string organizationUrl);
}
