namespace PersonalAssistant.Api.Configuration.Options;

public sealed class AiOptions
{
    public string BaseUrl { get; set; } = "http://localhost:11434";

    public string Model { get; set; } = "";
}