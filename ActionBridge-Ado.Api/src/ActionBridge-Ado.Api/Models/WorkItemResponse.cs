namespace ActionBridge_Ado.Api.Models;

public class WorkItemResponse
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public int Priority { get; set; }
}
