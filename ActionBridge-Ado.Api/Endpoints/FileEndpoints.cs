namespace ActionBridge_Ado.Api.Endpoints;

using ActionBridge_Ado.Api.Services.AI;
using ActionBridge_Ado.Api.Services.Ado;
using Microsoft.AspNetCore.Mvc;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/file");

        group.MapPost("/upload", async (
            IFormFile file,
            [FromQuery] string organization,
            [FromQuery] string project,
            IAIService aiService,
            IAdoService adoService) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("No file uploaded");

            using var stream = file.OpenReadStream();
            var workItems = await aiService.ParseFileToWorkItemsAsync(stream, file.FileName);

            // var results = new List<string>();
            // foreach (var workItem in workItems)
            // {
            //     var result = await adoService.CreateWorkItemAsync(organization, project, workItem);
            //     results.Add(result);
            // }

            // return Results.Ok(new { message = $"Created {results.Count} work items", workItems = results });
            return Results.Ok();
        }).DisableAntiforgery(); ;
    }
}