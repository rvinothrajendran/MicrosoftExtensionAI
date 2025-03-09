using System.ComponentModel;
using Microsoft.Extensions.AI;

namespace FunctionCall
{
    public enum Model
    {
        None,
        Ollama,
        AzureOpenAI
    }

    internal class Program
    {
       
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Function Call ");

            var selectedModel = InputHandler.GetModelSelection();

            IChatClient? client = ServiceHandler.GetService(selectedModel)!;

            Console.WriteLine($"\nModel Information {client.Metadata.ModelId} - {client.Metadata.ProviderName} - {client.Metadata.ProviderUri}\n");


            ChatOptions chatOptions = new ChatOptions()
            {
                Tools = new List<AITool>()
                {
                    AIFunctionFactory.Create(GetConversionRate)
                }
            };

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write("User > ");
                
                var userMessage = Console.ReadLine();

                if (userMessage == null) continue;

                var response = await client.CompleteAsync(userMessage,chatOptions);

                OutPutHandler.DisplayMessage(response);
            }

            return;
        }

        [Description("Get current conversation rate")]
        private static decimal? GetConversionRate(string sourceRates, string targetRates)
        {
            return sourceRates switch
            {
                "EUR" => targetRates switch
                {
                    "INR" => 88.50m,
                    "USD" => 1.10m,
                    "GBP" => 0.85m,
                    _ => null
                },
                "USD" => targetRates switch
                {
                    "INR" => 80.00m,
                    "EUR" => 0.91m,
                    "GBP" => 0.77m,
                    _ => null
                },
                "GBP" => targetRates switch
                {
                    "INR" => 102.50m,
                    "USD" => 1.30m,
                    "EUR" => 1.17m,
                    _ => null
                },
                "INR" => targetRates switch
                {
                    "EUR" => 0.011m,
                    "USD" => 0.012m,
                    "GBP" => 0.0098m,
                    _ => (decimal?)null
                },
                _ => null
            };
        }
        
    }
}
