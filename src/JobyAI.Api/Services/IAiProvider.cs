namespace JobyAI.Api.Services;

public interface IAiProvider
{
    Task<string> GenerateResponseAsync(string prompt);
}