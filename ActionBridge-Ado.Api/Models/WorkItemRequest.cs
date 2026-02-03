namespace ActionBridge_Ado.Api;

public class WorkItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Task"; // Task, Bug, User Story, etc.
    public string? Tags { get; set; }
    public string? AssignedTo { get; set; }
    public int? Priority { get; set; }
}
