namespace ActionBridge_Ado.Api.Services.Chunker;

public class TranscriptChunker
{
    public List<string> Chunk(string content, int targetTokens, int overlapTokens = 0)
    {
        int stepSize = targetTokens - overlapTokens;

        if (stepSize == 0 || overlapTokens > targetTokens || content == "")
        {
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
