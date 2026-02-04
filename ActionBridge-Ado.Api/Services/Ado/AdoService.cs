using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace ActionBridge_Ado.Api.Services.Ado;

public class AdoService : IAdoService
{
    private readonly IAuthService _authService;

    public AdoService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<List<WorkItem>> CreateWorkItemsAsync(string organizationUrl, string project, List<WorkItemRequest> workItems, VssOAuthAccessTokenCredential credentials)
    {
        var uri = new Uri(organizationUrl);
        var createdWorkItems = new List<WorkItem>();

        using var connection = new VssConnection(uri, credentials);
        using var witClient = connection.GetClient<WorkItemTrackingHttpClient>();

        foreach (var workItem in workItems)
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
                }
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
            //     patchDocument.Add(new JsonPatchOperation
            //     {
            //         Operation = Operation.Add,
            //         Path = "/fields/System.AssignedTo",
            //         Value = workItem.AssignedTo
            //     });
            // }

            try
            {
                var created = await witClient.CreateWorkItemAsync(patchDocument, project, workItem.Type);
                createdWorkItems.Add(created);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating work item '{workItem.Title}': {ex.Message}");
                throw;
            }
        }

        return createdWorkItems;
    }

    public async Task<IEnumerable<TeamProjectReference>> GetProjectsAsync(string organizationUrl)
    {
        var uri = new Uri(organizationUrl);

        var entraIdAccessToken = await _authService.GetAccessTokenAsync();
        var credentials = new VssOAuthAccessTokenCredential(entraIdAccessToken);

        using var connection = new VssConnection(uri, credentials);
        using var projectClient = connection.GetClient<ProjectHttpClient>();

        try
        {
            var projects = await projectClient.GetProjects();
            return projects;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving projects: {ex.Message}");
            throw;
        }
    }
}
