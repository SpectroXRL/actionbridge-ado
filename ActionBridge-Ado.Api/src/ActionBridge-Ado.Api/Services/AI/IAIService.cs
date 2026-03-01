using ActionBridge_Ado.Api.Models;

namespace ActionBridge_Ado.Api.Services.AI;

public interface IAIService
{
    Task<List<WorkItemRequest>> ParseFileToWorkItemsAsync(Stream fileStream, string fileName);
}
