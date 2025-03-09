using System.Text;
using Microsoft.Extensions.AI;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
namespace DemoEmbedding;

static class Program
{
    static async Task Main()
    {
        Console.WriteLine("Embedding");

        var endpoint = "http://localhost:11434/";
        var modelId = "mxbai-embed-large";
        
        var pdfPath = @"C:\\Users\\vinraj00\\Documents\\split-1.pdf";
        
        var pdfcontent = ReadPdfContent(pdfPath);

        Console.Read();
        
        IEmbeddingGenerator<string,Embedding<float>> generator = new OllamaEmbeddingGenerator(endpoint, modelId: modelId);
        
        var embedding = await generator.GenerateEmbeddingVectorAsync(pdfcontent);

        Console.Read();
    }
    
    private static async Task ChatWithPdf(string pdfPath)
    {
        var url = "http://localhost:11434/";
        var modelId = "llama3.2";

        IChatClient client = new OllamaChatClient(url, modelId);

        while (true)
        {
            var userMessage = Console.ReadLine();
            
            if (userMessage != null && 
                (userMessage.Equals("bye", StringComparison.OrdinalIgnoreCase) || userMessage.Equals("exit", StringComparison.OrdinalIgnoreCase)))
            {
                break;
            }

            var systemMessage = "your are helpful AI generate answers based on the pdf content :" +  pdfPath;    
            List<ChatMessage> messages =
            [
                new ChatMessage(ChatRole.System, systemMessage),
                new ChatMessage(ChatRole.User, userMessage)
            ];

            if (userMessage != null)
            {
                var streamResponse = client.CompleteStreamingAsync(messages);

                await foreach (var result in streamResponse)
                {
                    Console.Write(result.Text);
                }
            }
        }
    }

    private static string ReadPdfContent(string pdfPath)
    {
        StringBuilder text = new StringBuilder();
        
        using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath)))
        {
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)));
            }
        }
        
        return text.ToString();
    }
}