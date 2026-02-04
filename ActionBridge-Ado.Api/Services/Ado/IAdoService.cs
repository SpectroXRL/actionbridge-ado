using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.OAuth;

namespace ActionBridge_Ado.Api.Services.Ado;

public interface IAdoService
{
    Task<List<WorkItem>> CreateWorkItemsAsync(string organizationUrl, string project, List<WorkItemRequest> workItem, VssOAuthAccessTokenCredential credentials);
    Task<IEnumerable<TeamProjectReference>> GetProjectsAsync(string organizationUrl);
}
