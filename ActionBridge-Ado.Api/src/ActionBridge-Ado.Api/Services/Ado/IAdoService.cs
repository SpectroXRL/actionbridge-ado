using ActionBridge_Ado.Api.Models;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.OAuth;

namespace ActionBridge_Ado.Api.Services.Ado;

public interface IAdoService
{
    Task<WorkItemBatchResponse> CreateWorkItemsBatchAsync(string organizationUrl, string project, List<WorkItemRequest> workItems);
    Task<IEnumerable<TeamProjectReference>> GetProjectsAsync(string organizationUrl);
}
