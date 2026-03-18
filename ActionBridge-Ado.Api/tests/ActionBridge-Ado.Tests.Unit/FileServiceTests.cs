using System.Security.Cryptography;
using System.Text;
using ActionBridge_Ado.Api.Services.File;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Services.Common;

namespace ActionBridge_Ado.Tests.Unit;

public class FileServiceTests
{
    [Fact]
    public void IsValidExtension_ShouldReturnFalse_WhenExtensionNotAllowed()
    {
        // Arrange - just a string, no IFormFile needed!
        var fileService = new FileService();
        var fileName = "malware.exe";

        // Act
        var result = fileService.IsValidExtension(fileName);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ProcessDocx_ShouldReturnString_WhenDocumentValidType()
    {
        // Arrange
        var fileService = new FileService();
        using var stream = CreateTestDocx("Hello world");

        // Act
        var result = fileService.ProcessDocx(stream);

        // Assert
        result.Should().Be("Hello world");
    }

    private static MemoryStream CreateTestDocx(string content)
    {
        var stream = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
        {
            var body = new Body(new Paragraph(new Run(new Text(content))));
            doc.AddMainDocumentPart().Document = new Document(body);
        }
        stream.Position = 0;
        return stream;
    }

    [Fact]
    public async Task ReadContent_WhenStreamHasContent_ReturnsContentAsString()
    {
        // Arrange
        var fileService = new FileService();
        var content = "This is some test content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));  // How would you create a stream from a string?

        // Act
        var result = await fileService.ReadContentAsync(stream);

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public async Task ReadContent_WhenStreamIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var fileService = new FileService();
        var content = "";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));  // How would you create a stream from a string?

        // Act
        var result = await fileService.ReadContentAsync(stream);

        // Assert
        result.Should().Be(string.Empty);
    }
}
