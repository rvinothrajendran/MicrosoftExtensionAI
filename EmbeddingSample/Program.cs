using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using System.ClientModel;

namespace EmbeddingSample
{
    enum Model
    {
        None,
        Ollama,
        AzureOpenAI
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Embedding !!!");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please select the model to use:");
                Console.WriteLine("1. Ollama");
                Console.WriteLine("2. Azure OpenAI");
                Console.WriteLine("3. Exit");

                var model = Console.ReadLine();
                if (model == "3")
                {
                    break;
                }

                if (!Enum.TryParse<Model>(model, out var selectedModel))
                {
                    Console.WriteLine("Invalid model selected.");
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please enter the message to generate embedding for:");
                var userMessage = Console.ReadLine();

                IEmbeddingGenerator<string, Embedding<float>>? generator = GetEmbeddingGenerator(selectedModel);

                Console.ForegroundColor = ConsoleColor.Green;

                if (generator != null && userMessage != null)
                {
                    var embedding = await generator.GenerateAsync([userMessage]);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(string.Join(", ", embedding[0].Vector.ToArray()));
                }
                else
                {
                    Console.WriteLine("Failed to create embedding generator.");
                }

                Console.Read();
                Console.Clear();
            }

            Console.Read();

        }

        private static IEmbeddingGenerator<string, Embedding<float>>? GetEmbeddingGenerator(Model model)
        {

            switch (model)
            {
                case Model.Ollama:
                    return new OllamaEmbeddingGenerator(new Uri("http://localhost:11434/"), "mxbai-embed-large");
                    break;
                case Model.AzureOpenAI:

                    var key = Environment.GetEnvironmentVariable("GitHubToken", EnvironmentVariableTarget.User);

                    return new AzureOpenAIClient(new Uri("https://models.inference.ai.azure.com"),
                        new ApiKeyCredential(key)).AsEmbeddingGenerator("text-embedding-3-large");

                    break;
                default:
                    Console.WriteLine("Invalid model selected.");
                    break;
            }
            
            return null;
        }
    }
}
