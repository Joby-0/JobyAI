using System.ComponentModel;
using Permissions;

namespace Tools;

public class FileTools
{
    private readonly PermissionManager _permissionManager;

    public FileTools(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }
    [Description("Searches the entire computer for files. " + "Use this whenever the user asks to find, locate, search for, or look for a file.")]
    public string[] SearchFiles([Description("The text to search for in file names. " + "Examples: CV, JobySaaS, invoice, photo, report")] string searchTerm)
    {
        Console.WriteLine();
        Console.WriteLine(">>> SEARCHFILES TOOL WAS CALLED <<<");
        Console.WriteLine($"Searching entire computer for: {searchTerm}");
        Console.WriteLine();

        var results = new List<string>();

        foreach (var drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady)
                continue;

            var root = drive.RootDirectory.FullName;

            var permission = _permissionManager.GetAccessLevel(root);

            if (permission == PermissionLevel.None)
            {
                permission = _permissionManager.AskForAccess(root);
            }

            if (permission < PermissionLevel.Read)
                continue;

            SearchDirectory(root, searchTerm, results);

            if (results.Count >= 100)
                break;
        }

        return results.Take(100).ToArray();
    }

    private void SearchDirectory(string directory, string searchTerm, List<string> results)
    {
        if (results.Count >= 100)
            return;

        var permission = _permissionManager.GetAccessLevel(directory);

        if (permission < PermissionLevel.Read)
            return;

        try
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                if (Path.GetFileName(file).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(file);

                    if (results.Count >= 100)
                        return;
                }
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                SearchDirectory(subDirectory, searchTerm, results);

                if (results.Count >= 100)
                    return;
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip inaccessible directory.
        }
        catch (IOException)
        {
            // Skip unavailable directory.
        }
    }
}