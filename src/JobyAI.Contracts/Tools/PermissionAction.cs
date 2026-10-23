namespace JobyAI.Contracts.Tools;
public enum PermissionAction
{
    Allow,
    RequiresApproval,
    Deny
}

public sealed record PermissionDecision(PermissionAction Action, string? Reason = null);