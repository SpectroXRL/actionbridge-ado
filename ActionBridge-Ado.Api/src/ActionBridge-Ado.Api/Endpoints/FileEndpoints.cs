
using ActionBridge_Ado.Api.Models;
using ActionBridge_Ado.Api.Services.Ado;
using ActionBridge_Ado.Api.Services.AI;
using ActionBridge_Ado.Api.Services.Chunker;
using ActionBridge_Ado.Api.Services.File;
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
            IFileService fileService,
            ITranscriptChunker transcriptChunker,
            IAdoService adoService)
    {
        if (file == null || file.Length == 0)
            return TypedResults.BadRequest("No file uploaded");

        var fileStream = file.OpenReadStream();

        var content = await fileService.ReadContentAsync(fileStream);
        if (string.IsNullOrEmpty(content))
            return TypedResults.BadRequest("Empty file");

        var chunks = transcriptChunker.Chunk(content, 3000, 200);
        if (chunks.Count == 0)
            return TypedResults.BadRequest("Invalid chunking configuration");

        var workItems = await aiService.ProcessChunksAsync(chunks);

        // 4. Return results
        return TypedResults.Created(string.Empty, workItems);
    }
}