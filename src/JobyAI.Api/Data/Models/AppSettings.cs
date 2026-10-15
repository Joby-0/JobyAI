namespace JobyAI.Api.Data.Models;

public sealed class AppSettings
{
    public int Id { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTime UpdatedAt { get; set; }
}