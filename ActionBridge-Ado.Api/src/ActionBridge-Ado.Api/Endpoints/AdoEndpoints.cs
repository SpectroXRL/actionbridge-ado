using ActionBridge_Ado.Api.Models;
using ActionBridge_Ado.Api.Services.Ado;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ActionBridge_Ado.Api.Endpoints;

public static class AdoEndpoints
{
    public static void MapAdoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/ado")
            .RequireAuthorization();

        group.MapGet("/projects", GetProjects).DisableAntiforgery();
        group.MapPost("/workitems", CreateWorkItemsAsync).DisableAntiforgery();
    }

    private static async Task<Results<Created<WorkItemBatchResponse>, BadRequest<string>>> CreateWorkItemsAsync(
        [FromQuery] string organizationUrl,
        [FromQuery] string project,
        [FromBody] List<WorkItemRequest> workItems,
        IAdoService adoService,
        IAuthService authService)
    {
        try
        {
            var createdWorkItems = await adoService.CreateWorkItemsBatchAsync(
                organizationUrl,
                project,
                workItems);

            return TypedResults.Created(string.Empty, createdWorkItems);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Ok<IEnumerable<GetProjectsResponse>>> GetProjects([FromQuery] string organizationUrl, IAdoService adoService)
    {
        var projects = await adoService.GetProjectsAsync(organizationUrl);

        return TypedResults.Ok(projects);
    }
}