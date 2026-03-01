
using ActionBridge_Ado.Api.Models;
using ActionBridge_Ado.Api.Services.Ado;
using ActionBridge_Ado.Api.Services.AI;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ActionBridge_Ado.Api.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/file")
        .RequireAuthorization();

        group.MapPost("/upload", UploadFileAsync).DisableAntiforgery(); ;
    }

    private static async Task<Results<Created<List<WorkItemRequest>>, BadRequest<string>>> UploadFileAsync(IFormFile file,
            IAIService aiService,
            IAdoService adoService)
    {
        if (file == null || file.Length == 0)
            return TypedResults.BadRequest("No file uploaded");


        using var stream = file.OpenReadStream();
        var workItems = await aiService.ParseFileToWorkItemsAsync(stream, file.FileName);

        return TypedResults.Created(string.Empty, workItems);
    }
}