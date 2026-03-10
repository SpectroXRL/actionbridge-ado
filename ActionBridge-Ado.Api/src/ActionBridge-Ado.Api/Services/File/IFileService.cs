using ActionBridge_Ado.Api.Models;

namespace ActionBridge_Ado.Api.Services.FIle;

public interface IFileService
{
    bool IsValidExtension(string fileName);
    string ProcessDocx(Stream fileStream);
}
