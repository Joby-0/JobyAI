using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Permissions;
using UglyToad.PdfPig;

namespace Tools;

using System.ComponentModel;
using System.Diagnostics;
using Permissions;


public class ApplicationTools
{
    private readonly PermissionManager _permissionManager;

    public ApplicationTools(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    [Description(
        "Searches for installed Windows applications and games by name. " +
        "Use this when the user asks to find, open, launch, or start an application or game. " +
        "Do not guess the application's path."
    )]
    public string[] SearchApplications(
        [Description(
            "The name or main part of the application or game name. " +
            "Examples: Chrome, Spotify, Steam, Minecraft, Discord."
        )]
        string searchTerm)
    {
        var results = new List<string>();

        if (string.IsNullOrWhiteSpace(searchTerm))
            return [];

        var locations = GetApplicationLocations();

        foreach (var location in locations)
        {
            SearchDirectory(location, searchTerm, results);

            if (results.Count >= 50)
                break;
        }

        return results.ToArray();
    }

    [Description(
        "Opens or launches a Windows application or game using its executable file. " +
        "Only use this after SearchApplications has found a valid application path. " +
        "Never invent an application path."
    )]
    public string OpenApplication([Description("The full path to the application's executable file.")] string applicationPath)
    {
        if (string.IsNullOrWhiteSpace(applicationPath))
            return "No application path was provided.";

        if (!File.Exists(applicationPath))
            return $"Application not found: {applicationPath}";

        var directory = Path.GetDirectoryName(applicationPath);

        if (directory is null)
            return "Could not determine the application's directory.";

        var permission = _permissionManager.GetAccessLevel(applicationPath);

        if (permission == PermissionLevel.None)
        {
            permission = _permissionManager.AskForAccess(directory);
        }

        if (permission is PermissionLevel.None or PermissionLevel.Denied)
            return $"I don't have permission to open: {applicationPath}";

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = applicationPath,
                UseShellExecute = true
            });

            return $"Successfully launched: {applicationPath}";
        }
        catch (Exception ex)
        {
            return $"Could not launch application: {ex.Message}";
        }
    }

    private static IEnumerable<string> GetApplicationLocations()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var locations = new[]
        {
            @"C:\Program Files",
            @"C:\Program Files (x86)",

            Path.Combine(userProfile, @"AppData\Local"),

            Path.Combine(userProfile, @"AppData\Roaming"),

            Path.Combine(userProfile, @"AppData\Local\Programs"),

            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs"),

            Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };

        return locations.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static void SearchDirectory(string directory, string searchTerm, List<string> results)
    {
        if (results.Count >= 50)
            return;

        try
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);

                if (!fileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(file);

                    if (results.Count >= 50)
                        return;
                }
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                SearchDirectory(
                    subDirectory,
                    searchTerm,
                    results);

                if (results.Count >= 50)
                    return;
            }
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
    }
}