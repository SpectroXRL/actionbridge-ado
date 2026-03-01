
using System.Text.Json;
using ActionBridge_Ado.Api.Models;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace ActionBridge_Ado.Api.Services.AI;

public class AIService : IAIService
{
    private readonly AzureOpenAIOptions _options;
    private readonly AzureOpenAIClient _client;

    public AIService(IOptions<AzureOpenAIOptions> options)
    {
        _options = options.Value;
        _client = new AzureOpenAIClient(
            new Uri(_options.Endpoint),
            new AzureKeyCredential(_options.ApiKey));
    }

    public async Task<List<WorkItemRequest>> ParseFileToWorkItemsAsync(Stream fileStream, string fileName)
    {
        // Read file content
        using var reader = new StreamReader(fileStream);
        var fileContent = await reader.ReadToEndAsync();

        var chatClient = _client.GetChatClient(_options.DeploymentName);

        var systemPrompt = @"
You are an assistant that converts documents into Azure DevOps work items.
Analyze the provided content and generate a list of work items.

Each work item should have:
- Title: A concise title for the task/story/bug
- Description: Detailed description of the work
- Type: One of 'Task', 'User Story', 'Bug', 'Epic', or 'Feature'
- Tags: Comma-separated relevant tags (optional)
- Priority: 1 (Critical), 2 (High), 3 (Medium), or 4 (Low)
- AcceptanceCriteria: Clear criteria for completion (optional)

Return a JSON object with a 'workItems' array.";

        var userPrompt = $"File name: {fileName}\n\nContent:\n{fileContent}";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        // Define the JSON schema for guaranteed structured output
        var jsonSchema = BinaryData.FromBytes("""
        {
            "type": "object",
            "properties": {
                "workItems": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "Title": { "type": "string" },
                            "Description": { "type": "string" },
                            "Type": { "type": "string", "enum": ["Task", "Epic", "Issue"] },
                            "Tags": { "type": "string" },
                            "Priority": { "type": "integer", "enum": [1, 2, 3, 4] }
                        },
                        "required": ["Title", "Description", "Type", "Tags", "Priority"],
                        "additionalProperties": false
                    }
                }
            },
            "required": ["workItems"],
            "additionalProperties": false
        }
        """u8.ToArray());

        var options = new ChatCompletionOptions
        {
            Temperature = 0.3f,
            MaxOutputTokenCount = 4000,
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "work_items_response",
                jsonSchema: jsonSchema,
                jsonSchemaIsStrict: true)
        };

        try
        {
            var response = await chatClient.CompleteChatAsync(messages, options);
            var responseContent = response.Value.Content[0].Text;

            // Parse the structured response
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var workItemsJson = jsonResponse.GetProperty("workItems").GetRawText();

            var workItems = JsonSerializer.Deserialize<List<WorkItemRequest>>(workItemsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return workItems ?? new List<WorkItemRequest>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI parsing failed: {ex.Message}");
            throw;
        }
    }
}
