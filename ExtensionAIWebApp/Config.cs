namespace ExtensionAIWebApp;

public static class Config
{
    // GitHub Configurations
    public static string GitHubEndPoint => "https://models.inference.ai.azure.com";

    public static string GitHubToken
        => Environment.GetEnvironmentVariable("GitHubToken", EnvironmentVariableTarget.User)
           ?? throw new InvalidOperationException("GitHubToken environment variable is not set.");

    public static string GitHubModel => "gpt-4o-mini";

}