using System.Text.Json;
using System.Text;
using Microsoft.Extensions.AI;

namespace ConsoleWebAppTest
{
    internal class Program
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        static async Task Main(string[] args)
        {
            Console.WriteLine("Test WebAPI Microsoft ExtensionAI");

            string apiUrl = "https://localhost:7201/chat";

            Console.WriteLine("Enter a message to send to the chatbot");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;

                var userMessage = Console.ReadLine();

                if (userMessage != null)
                {
                  var response = await CallApiAsync(apiUrl, userMessage);
                  
                  Console.ForegroundColor = ConsoleColor.Green;

                  Console.WriteLine(response?.Message.Text);
                }
            }


            Console.ReadLine();
        }

        private static async Task<ChatCompletion?> CallApiAsync(string url, string input)
        {
            using HttpClient client = new HttpClient();
            var json = JsonSerializer.Serialize(input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChatCompletion>(responseContent, JsonOptions);
        }
    }
}
