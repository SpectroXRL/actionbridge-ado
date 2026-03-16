using ActionBridge_Ado.Api.Services.Chunker;
using FluentAssertions;

namespace ActionBridge_Ado.Tests.Unit;

public class TranscriptChunkerTests
{
    [Fact]
    public void Chunk_WhenContentSmallerThanTargetSize_ReturnsSingleChunk()
    {
        // Arrange
        var chunker = new TranscriptChunker();
        var content = "This is a short transcript.";
        var targetTokens = 1000;

        // Act
        var result = chunker.Chunk(content, targetTokens);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be("This is a short transcript.");
    }

    [Fact]
    public void Chunk_WhenOverlapEqualsTargetTokens_HandlesGracefully()
    {
        // Arrange
        var chunker = new TranscriptChunker();
        var content = "one two three four five six seven eight nine ten";
        var targetTokens = 5;
        var overlapTokens = 5;  // Same as target!

        // Act
        var result = chunker.Chunk(content, targetTokens, overlapTokens);

        // Assert
        // 🤔 YOUR TURN: What should happen here?
        // 
        // Think about: stepSize = targetTokens - overlapTokens = 5 - 5 = 0
        // A step size of 0 means... what happens in your loop?
        // What SHOULD the behavior be to avoid infinite loop or crash?
        result.Should().HaveCount(0);
    }

    [Fact]
    public void Chunk_WhenContentIsEmpty_ReturnsAppropriateResult()
    {
        // Arrange
        var chunker = new TranscriptChunker();
        var content = "";
        var targetTokens = 5;

        // Act
        var result = chunker.Chunk(content, targetTokens);

        // Assert
        // 🤔 YOUR TURN: What should an empty string return?
        // 
        // Think about: content.Split(' ') on "" gives what?
        // Should we return an empty list? A list with one empty string?
        // What would be most useful for the caller (AIService)?
        result.Should().HaveCount(0);
    }

    [Fact]
    public void Chunk_WhenContentExceedsTargetSize_ReturnsMultipleChunks()
    {
        // Arrange
        var chunker = new TranscriptChunker();

        // Create content that's clearly larger than target
        // If we assume ~1 token per word, 10 words ≈ 10 tokens
        var content = "one two three four five six seven eight nine ten";
        var targetTokens = 5;  // Force a split

        // Act
        var result = chunker.Chunk(content, targetTokens);

        // Assert
        result.Should().HaveCountGreaterThan(1);
        result[0].Should().Be("one two three four five");
        result[1].Should().Be("six seven eight nine ten");
    }

    [Fact]
    public void Chunk_WithOverlap_ReturnsOverlappingChunks()
    {
        // Arrange
        var chunker = new TranscriptChunker();
        var content = "one two three four five six seven eight nine ten";
        var targetTokens = 5;
        var overlapTokens = 2;

        // Act
        var result = chunker.Chunk(content, targetTokens, overlapTokens);

        // Assert
        // What should result contain?
        // Think about: How many chunks? What's in each?
        result.Should().HaveCountGreaterThan(1);
        result[0].Should().Be("one two three four five");
        result[1].Should().Be("four five six seven eight");
        result[2].Should().Be("seven eight nine ten");
    }
}
