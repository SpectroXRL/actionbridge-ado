using ActionBridge_Ado.Api.Services.FIle;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ActionBridge_Ado.Api.Services.File;

public class FileService : IFileService
{
    private static readonly string[] AllowedExtensions = [".txt", ".docx", ".vtt"];
    public FileService()
    {

    }

    public bool IsValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    public string ProcessDocx(Stream fileStream)
    {
        using var doc = WordprocessingDocument.Open(fileStream, false);
        var body = doc.MainDocumentPart?.Document?.Body;

        if (body == null)
        {
            return string.Empty;
        }

        var paragraphs = body.Elements<Paragraph>().Select(p => p.InnerText);

        return string.Join(Environment.NewLine, paragraphs);
    }
}
