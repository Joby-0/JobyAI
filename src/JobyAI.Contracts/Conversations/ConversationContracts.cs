namespace JobyAI.Contracts.Conversations;

public sealed record ConversationSummary(
    Guid Id,
    string? Title,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record ConversationDetails(
    Guid Id,
    string? Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ConversationMessage> Messages);

public sealed record ConversationMessage(
    Guid Id,
    string? Role,
    string? Content,
    DateTimeOffset CreatedAt);
