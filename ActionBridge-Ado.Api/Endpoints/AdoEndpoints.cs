namespace ActionBridge_Ado.Api.Endpoints;

using ActionBridge_Ado.Api.Services.Ado;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Services.OAuth;

public static class AdoEndpoints
{
    public static void MapAdoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/ado")
            .RequireAuthorization();

        group.MapGet("/projects", async (
            [FromQuery] string organizationUrl,
            IAdoService adoService) =>
        {
            var projects = await adoService.GetProjectsAsync(organizationUrl);

            return TypedResults.Ok(projects);
        }).DisableAntiforgery();

        group.MapPost("/workitems", CreateWorkItemsAsync).DisableAntiforgery();
    }

    private static async Task<Results<Created<object>, BadRequest<object>>> CreateWorkItemsAsync(
        [FromQuery] string organizationUrl,
        [FromQuery] string project,
        [FromBody] List<WorkItemRequest> workItems,
        IAdoService adoService,
        IAuthService authService)
    {
        // Get Microsoft Entra ID access token
        var entraIdAccessToken = await authService.GetAccessTokenAsync();
        var credentials = new VssOAuthAccessTokenCredential(entraIdAccessToken);

        try
        {
            var createdWorkItems = await adoService.CreateWorkItemsAsync(
                organizationUrl,
                project,
                workItems,
                credentials);

            var response = createdWorkItems.Select(w => new
            {
                id = w.Id,
                title = w.Fields["System.Title"]?.ToString(),
                type = w.Fields["System.WorkItemType"]?.ToString(),
                url = w.Url
            });

            return TypedResults.Created(string.Empty, (object)new
            {
                created = createdWorkItems.Count,
                total = workItems.Count,
                workItems = response,
                errors = new List<string>()
            });
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest((object)new
            {
                created = 0,
                total = workItems.Count,
                workItems = new List<object>(),
                errors = new List<string> { ex.Message }
            });
        }
    }
}