using System;
using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;

namespace ImageExtensionAI.UI;

public sealed class LLMClient
{
    public static IChatClient CreateOllamaClient()
    {
        IChatClient? client = new OllamaChatClient(new Uri("http://localhost:11434/"), "llava")
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
        return client;
    }

    public static IChatClient? CreateAzureClient()
    {
        var key = Environment.GetEnvironmentVariable("GitHubToken", EnvironmentVariableTarget.User);
        if (key != null)
        {
            var credential = new AzureKeyCredential(key);

            IChatClient? client =
                new ChatCompletionsClient(new Uri("https://models.inference.ai.azure.com"), credential).AsChatClient("gpt-4o-mini")
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build();
            return client;
        }
        return null;
    }
}