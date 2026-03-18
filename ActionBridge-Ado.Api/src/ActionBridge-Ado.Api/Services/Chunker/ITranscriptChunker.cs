using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActionBridge_Ado.Api.Services.Chunker
{
    public interface ITranscriptChunker
    {
        List<string> Chunk(string content, int targetTokens, int overlapTokens);
    }
}