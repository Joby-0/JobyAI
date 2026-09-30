using System.Text.Json;

namespace Permissions;

public class PermissionManager
{
    private readonly string _filePath;
    private FilePermissions _permissions;

    public PermissionManager(string filePath)
    {
        _filePath = filePath;
        _permissions = Load();
    }

    private FilePermissions Load()
    {
        if (!File.Exists(_filePath))
            return new FilePermissions();

        var json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<FilePermissions>(json) ?? new FilePermissions();
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(
            _permissions,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, json);

        Console.WriteLine("Permission saved");
    }

    public PermissionLevel AskForAccess(string directory)
    {
        Console.WriteLine();
        Console.WriteLine($"I need access to {directory} to find what you're looking for!");
        Console.WriteLine("[Y] Full    [R] Read only    [W] Write and read    [N] Denied    [A] Not now");

        while (true)
        {
            var answer = Console.ReadKey(true).Key;

            switch (answer)
            {
                case ConsoleKey.Y:
                    GrantAccess(directory, PermissionLevel.Full);
                    return PermissionLevel.Full;

                case ConsoleKey.R:
                    GrantAccess(directory, PermissionLevel.Read);
                    return PermissionLevel.Read;

                case ConsoleKey.W:
                    GrantAccess(directory, PermissionLevel.Write);
                    return PermissionLevel.Write;

                case ConsoleKey.N:
                    GrantAccess(directory, PermissionLevel.Denied);
                    return PermissionLevel.Denied;

                case ConsoleKey.A:
                    return PermissionLevel.None;

                default:
                    Console.WriteLine();
                    Console.WriteLine("Please choose Y, R, W, N or A.");
                    break;
            }
        }
    }
    public void GrantAccess(string directory, PermissionLevel level)
    {
        var existing = _permissions.Permissions
            .FirstOrDefault(p =>
                p.Directory.Equals(
                    directory,
                    StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            existing.Level = level;
        }
        else
        {
            _permissions.Permissions.Add(
                new FilePermission
                {
                    Directory = directory,
                    Level = level
                });
        }

        Save();
    }

    public PermissionLevel GetAccessLevel(string path)
    {
        var permission = _permissions.Permissions.FirstOrDefault(permission => path.Equals(permission.Directory, StringComparison.OrdinalIgnoreCase) ||
         path.StartsWith(permission.Directory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));

        return permission?.Level ?? PermissionLevel.None;
    }
}