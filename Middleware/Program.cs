using Microsoft.Extensions.AI;

namespace Middleware
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, MiddleWare!");

            IChatClient? client = CreateOllamaClient();

            var response = await client.CompleteAsync("Hello");

            Console.WriteLine(response);

            Console.Read();
        }

        private static IChatClient CreateOllamaClient()
        {
            IChatClient client = new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2")
                .AsBuilder()
                //.Use(middleware => new HistoricalEventsChatClient(middleware))
                .UseHistoricalEvent()
                .Build();
            return client;
        }
    }

    public static class HistoricalMiddleware
    {
        public static ChatClientBuilder UseHistoricalEvent(this ChatClientBuilder builder)
        {
            return builder.Use(middleware => new HistoricalEventsChatClient(middleware));
        }
    }

    public class HistoricalEventsChatClient(IChatClient chatClient) : DelegatingChatClient(chatClient)
    {
        public override async Task<ChatCompletion> CompleteAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {

            var date = DateTime.Now.ToString("dd");
            var month = DateTime.Now.ToString("MM");

            var historyPrompt =
                $"Today's date : {date} and Month is : {month}. Can you provide any notable historical events or special occurrences associated with this date or month?";


            var chatPromptMessage = new ChatMessage(ChatRole.User, historyPrompt);

            chatMessages.Add(chatPromptMessage);

            try
            {
                return await base.CompleteAsync(chatMessages, options, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                chatMessages.Remove(chatPromptMessage);
            }

            return null;
        }

        public override IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return base.CompleteStreamingAsync(chatMessages, options, cancellationToken);
        }
    }
}
