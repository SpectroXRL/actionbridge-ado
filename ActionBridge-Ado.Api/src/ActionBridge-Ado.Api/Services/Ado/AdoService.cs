using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using ActionBridge_Ado.Api.Models;

namespace ActionBridge_Ado.Api.Services.Ado;

public class AdoService : IAdoService
{
    private readonly IAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdoService> _logger;

    public AdoService(IAuthService authService, IHttpClientFactory httpClientFactory, ILogger<AdoService> logger)
    {
        _authService = authService;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<WorkItemBatchResponse> CreateWorkItemsBatchAsync(
        string organizationUrl,
        string project,
        List<WorkItemRequest> workItems)
    {
        // Get the token from credentials
        var token = await _authService.GetAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var batchRequests = workItems.Select((workItem, index) => new WorkItemBatchRequest
        {
            Method = "PATCH",
            Uri = $"/{project}/_apis/wit/workitems/${Uri.EscapeDataString(workItem.Type)}?api-version=7.1",
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json-patch+json" }
            },
            Body = BuildPatchDocument(workItem, index)
        }).ToList();

        var json = JsonConvert.SerializeObject(batchRequests);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogDebug("Batched {WorkItemNumber} of work items", batchRequests.Count);
        var response = await _httpClient.PostAsync(
            $"{organizationUrl}/_apis/wit/$batch?api-version=1.0",
            content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Batch request failed: {StatusCode} - {Error}", response.StatusCode, error);
            throw new Exception($"Batch request failed: {response.StatusCode} - {error}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        var batchResponse = JsonConvert.DeserializeObject<WorkItemBatchResponse>(responseBody);

        if (batchResponse != null)
        {
            _logger.LogInformation("Created {ADOCount} of work items in Azure DevOps", batchResponse.Count);
        }
        else
        {
            _logger.LogWarning("No Azure DevOps work items were created");
        }

        return batchResponse ?? new WorkItemBatchResponse();
    }

    private JsonPatchDocument BuildPatchDocument(WorkItemRequest workItem, int index)
    {
        var patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = workItem.Title
                },
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Description",
                    Value = workItem.Description
                },
                new JsonPatchOperation
                {
                    Operation = Operation.Add,
                    Path = "/id",
                    Value = $"{(index + 1) * -1}"
                },
            };

        if (!string.IsNullOrEmpty(workItem.Tags))
        {
            patchDocument.Add(new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = "/fields/System.Tags",
                Value = workItem.Tags
            });
        }

        if (workItem.Priority.HasValue)
        {
            patchDocument.Add(new JsonPatchOperation
            {
                Operation = Operation.Add,
                Path = "/fields/Microsoft.VSTS.Common.Priority",
                Value = workItem.Priority.Value
            });
        }

        // if (!string.IsNullOrEmpty(workItem.AssignedTo))
        // {
        //     patchDoc.Add(new { op = "add", path = "/fields/System.AssignedTo", value = workItem.AssignedTo });
        // }
        return patchDocument;
    }

    public async Task<IEnumerable<GetProjectsResponse>> GetProjectsAsync(string organizationUrl)
    {
        var uri = new Uri(organizationUrl);

        var entraIdAccessToken = await _authService.GetAccessTokenAsync();
        var credentials = new VssOAuthAccessTokenCredential(entraIdAccessToken);

        using var connection = new VssConnection(uri, credentials);
        using var projectClient = connection.GetClient<ProjectHttpClient>();

        try
        {
            var projects = await projectClient.GetProjects();
            _logger.LogInformation("Retrieved {ProjectCount} number of projects", projects.Count);

            var projectResponse = projects.Select(project => new GetProjectsResponse() { Id = project.Id, Name = project.Name });
            return projectResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            throw;
        }
    }
}