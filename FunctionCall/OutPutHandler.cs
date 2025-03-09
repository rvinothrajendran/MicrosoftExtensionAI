using Microsoft.Extensions.AI;

namespace FunctionCall;

public static class OutPutHandler
{
    public static void DisplayMessage(ChatCompletion response)
    {
        Console.ForegroundColor = ConsoleColor.White;

        Console.Write("User > ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n");
        Console.Write("\nBot > ");
        Console.Write(response.Message.Text);
        Console.WriteLine("\n");
    }
}