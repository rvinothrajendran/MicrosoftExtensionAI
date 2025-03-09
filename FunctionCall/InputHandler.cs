namespace FunctionCall;

public static class InputHandler
{
    public static Model GetModelSelection()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Please select the model to use:");
        Console.WriteLine("1. Ollama");
        Console.WriteLine("2. Azure OpenAI");
        Console.WriteLine("3. Exit");

        var modelSelection = Console.ReadLine();

        if (modelSelection == "3")
        {
            Console.WriteLine("Exiting...");
            return Model.None; // Return null to indicate exit
        }

        if (!Enum.TryParse<Model>(modelSelection, out Model selectedModel))
        {
            Console.WriteLine("Invalid model selected.");
            return Model.None; // Return null if invalid selection
        }

        return selectedModel; // Return the selected model if valid
    }
}