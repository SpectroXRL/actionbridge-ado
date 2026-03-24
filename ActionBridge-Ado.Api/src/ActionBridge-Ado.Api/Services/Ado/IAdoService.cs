using ActionBridge_Ado.Api.Models;

namespace ActionBridge_Ado.Api.Services.Ado;

public interface IAdoService
{
    Task<WorkItemBatchResponse> CreateWorkItemsBatchAsync(string organizationUrl, string project, List<WorkItemRequest> workItems);
    Task<IEnumerable<GetProjectsResponse>> GetProjectsAsync(string organizationUrl);
    Task<IEnumerable<GetOrganizationsResponse>> GetOrganizationsAsync();
}
