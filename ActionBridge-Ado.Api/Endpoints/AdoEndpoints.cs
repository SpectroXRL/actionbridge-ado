namespace ActionBridge_Ado.Api.Endpoints;

using ActionBridge_Ado.Api.Services.Ado;
using Microsoft.AspNetCore.Mvc;

public static class AdoEndpoints
{
    public static void MapAdoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/ado");

        group.MapGet("/projects", async (
            [FromQuery] string organizationUrl,
            IAdoService adoService) =>
        {
            var projects = await adoService.GetProjectsAsync(organizationUrl);

            return Results.Ok(projects);
        }).DisableAntiforgery();
    }
}