namespace ActionBridge_Ado.Api.Services.Chunker;

public class TranscriptChunker : ITranscriptChunker
{
    private readonly ILogger<TranscriptChunker> _logger;

    public TranscriptChunker(ILogger<TranscriptChunker> logger)
    {
        _logger = logger;
    }

    public List<string> Chunk(string content, int targetTokens, int overlapTokens = 0)
    {
        int stepSize = targetTokens - overlapTokens;

        if (stepSize == 0 || overlapTokens > targetTokens || content == "")
        {
            _logger.LogWarning("Chunk failed with StepSize: {StepSize}; ContentLength: {ContentLength}", stepSize, content.Length);
            return new List<string>();
        }

        var words = content.Split(' ');
        List<string> finalContent = new List<string>();

        for (int i = 0; i < words.Length; i += stepSize)
        {
            var chunk = words[i..Math.Min(i + targetTokens, words.Length)];
            finalContent.Add(string.Join(" ", chunk));
        }

        return finalContent;
    }
}
