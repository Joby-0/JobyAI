namespace JobyAI.Api.Configuration.Options;

public sealed class AiOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";

    public string Model { get; set; } = "";

    public int RequestTimeoutSeconds { get; set; } = 300;

    public int HealthCheckTimeoutSeconds { get; set; } = 5;
}