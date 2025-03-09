using Azure;
using Azure.AI.Inference;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace ExtensionAIWebApp
{
    public class Program
    {

        static string url = "http://localhost:11434/";
        private static string modelId = "llama3.2";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(new OllamaChatClient(url, modelId));

            builder.Services.AddChatClient(services => services.GetRequiredService<OllamaChatClient>());

            var token = Environment.GetEnvironmentVariable("GitHubToken",EnvironmentVariableTarget.User);

            if (token != null)
            {
                builder.Services.AddSingleton(new ChatCompletionsClient(
                    new Uri("https://models.inference.ai.azure.com"),
                    new AzureKeyCredential(token)));

                builder.Services.AddChatClient(services =>
                    services.GetRequiredService<ChatCompletionsClient>().AsChatClient("gpt-4o-mini"));
            }

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.MapPost("/chat", async (IChatClient client, [FromBody] string message) =>
            await client.CompleteAsync(message));
            
            app.Run();
        }
    }
}
