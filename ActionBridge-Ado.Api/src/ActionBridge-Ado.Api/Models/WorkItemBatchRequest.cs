using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
namespace ActionBridge_Ado.Api.Models;

public class WorkItemBatchRequest
{
    public string Method { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = [];
    public JsonPatchDocument Body { get; set; } = [];
}
