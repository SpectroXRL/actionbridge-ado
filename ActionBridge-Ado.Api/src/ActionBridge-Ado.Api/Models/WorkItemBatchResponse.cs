namespace ActionBridge_Ado.Api.Models;

public class WorkItemBatchResponse
{
    public int Count { get; set; } = 0;
    public List<WorkItemBatchResponseValue> Value { get; set; } = [];
}
