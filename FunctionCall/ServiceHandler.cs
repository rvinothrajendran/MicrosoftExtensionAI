using Azure.AI.Inference;
using Azure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionCall;

public static class ServiceHandler
{
    private static ServiceProvider? _serviceProvider;
    private static void CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton(provider => new Dictionary<Model, IChatClient>()
        {
            {
                Model.AzureOpenAI, CreateAzureClient()
            },
            {
                Model.Ollama, CreateOllamaClient()
            }
        });

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public static IChatClient? GetService(Model model)
    {
        if (_serviceProvider == null)
            CreateServiceCollection();

        IChatClient? chatClient = null;

        _serviceProvider?.GetRequiredService<Dictionary<Model, IChatClient>>().TryGetValue(model, out chatClient);
        
        return chatClient;

    }

    private static IChatClient CreateAzureClient()
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

    private static IChatClient CreateOllamaClient()
    {
        IChatClient? client = new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2")
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();
        return client;
    }

}
