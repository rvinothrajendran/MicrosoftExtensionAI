using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.Text.Json;
using ChatRole = Microsoft.Extensions.AI.ChatRole;

namespace HelloExtensionAI;

internal static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, Microsoft Extensions AI Services!");

        var prompt = "Write an email to the .NET team, thanking them for creating the wonderful AI library";

        // GitHub Model
        IChatClient client =
            new ChatCompletionsClient(new Uri(Config.GitHubEndPoint), new AzureKeyCredential(Config.GitHubToken))
                .AsChatClient(Config.GitHubModel);

        // Azure Model
        // IChatClient client =
        //     new AzureOpenAIClient(new Uri(Config.AzureOpenAIEndPoint), new ApiKeyCredential(Config.AzureOpenAIKey))
        //         .AsChatClient(Config.AzureOpenAIModel);


        // Send the prompt to the model
        var response = await client.CompleteAsync(prompt);

        Console.WriteLine(response.Message.Text);

        Console.Read();

        Console.WriteLine("Stream Response");

        var streamResponse = client.CompleteStreamingAsync(prompt);

        await foreach (var result in streamResponse)
        {
            Console.Write(result.Text);
        }

        Console.Read();
    }
}
