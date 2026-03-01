namespace ActionBridge_Ado.Api.Models;

public class WorkItemBatchResponseValue
{
    public int Code { get; set; }
    public Dictionary<string, string> Headers { get; set; } = [];
    public string Body { get; set; } = string.Empty;
}
