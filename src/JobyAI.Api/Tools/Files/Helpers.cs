using JobyAI.Api.Security.Permissions;

namespace JobyAI.Api.Tools;

public class FileSearchHelper
{
    private readonly PermissionManager _permissionManager = new PermissionManager("permissions.json");
    private void SearchDirectory(string directory, string searchTerm, List<string> results, ref int filesSearched)
    {
        // Limit the number of results to 100 to avoid overwhelming the user.
        if (results.Count >= 100)
            return;

        if (ShouldSkipDirectory(directory))
            return;

        var permission = _permissionManager.GetAccessLevel(directory);

        // Console.WriteLine($"Checking: {directory} | Permission: {permission}");
        Console.WriteLine($"\r Searching: {directory}... Files searched: {filesSearched} | Results found: {results.Count}");
        if (permission < PermissionLevel.Read)
            return;

        try
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                filesSearched++;

                if (Path.GetFileName(file).Contains(
                        searchTerm,
                        StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(file);

                    if (results.Count >= 100)
                        return;
                }
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                SearchDirectory(subDirectory, searchTerm, results, ref filesSearched);

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

    public bool ShouldSkipDirectory(string directory)
    {
        var normalized = directory.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar);

        return normalized.StartsWith(
                   @"C:\Windows",
                   StringComparison.OrdinalIgnoreCase)
               || normalized.StartsWith(
                   @"C:\Program Files",
                   StringComparison.OrdinalIgnoreCase)
               || normalized.StartsWith(
                   @"C:\ProgramData",
                   StringComparison.OrdinalIgnoreCase)
               || normalized.Contains(
                   @"\$Recycle.Bin",
                   StringComparison.OrdinalIgnoreCase)
               || normalized.Contains(
                   @"\System Volume Information",
                   StringComparison.OrdinalIgnoreCase);
    }
}
