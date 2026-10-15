namespace Permissions;

public class FilePermission
{
    public string Directory { get; set; } = string.Empty;

    public PermissionLevel Level { get; set; }
}
public class FilePermissions
{
    public List<FilePermission> Permissions { get; set; } = [];
}

