using ActionBridge_Ado.Api.Models;

namespace ActionBridge_Ado.Api.Services.AI;

public interface IAIService
{
    Task<List<WorkItemRequest>> ProcessChunksAsync(List<string> chunks);
}
