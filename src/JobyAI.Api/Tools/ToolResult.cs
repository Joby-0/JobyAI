using System.Text.Json;

namespace JobyAI.Api.Tools;

public sealed record ToolResult(bool Succeeded, string Message, JsonElement? Data = null, string? ErrorCode = null)
{
    public static ToolResult Success(string message, object? data = null)
    {
        return new ToolResult(true, message, data is null ? null : JsonSerializer.SerializeToElement(data));
    }

    public static ToolResult Failure(string errorCode, string message)
    {
        return new ToolResult(false, message, ErrorCode: errorCode);
    }
}