namespace HelloExtensionAI;

public static class Config
{
    // GitHub Configurations
    public static string GitHubEndPoint => "https://models.inference.ai.azure.com";

    public static string GitHubToken
        => Environment.GetEnvironmentVariable("GitHubToken", EnvironmentVariableTarget.User)
           ?? throw new InvalidOperationException("GitHubToken environment variable is not set.");

    public static string GitHubModel => "gpt-4o-mini";


    // Azure OpenAI Configurations 
    public static string AzureOpenAIKey => Environment.GetEnvironmentVariable("AzureOpenAIKey", EnvironmentVariableTarget.User)
                                           ?? throw new InvalidOperationException("AzureOpenAIKey environment variable is not set.");

    public static string AzureOpenAIEndPoint => Environment.GetEnvironmentVariable("AzureOpenAIEndPoint", EnvironmentVariableTarget.User)
                                                ?? throw new InvalidOperationException("AzureOpenAIEndPoint environment variable is not set.");

    public static string AzureOpenAIModel => Environment.GetEnvironmentVariable("AzureOpenAIModel", EnvironmentVariableTarget.User)
                                             ?? throw new InvalidOperationException("AzureOpenAIModel environment variable is not set.");
}