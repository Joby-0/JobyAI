namespace JobyAI.Api.Data.Models;

public sealed class AppSettings
{
    public int Id { get; set; }
    public string? Key { get; set; }
    public string Value { get; set; }
    public DateTime UpdatedAt { get; set; }
}