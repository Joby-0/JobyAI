using System.ComponentModel;

namespace Tools;

public class FileTools
{
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

            SearchDirectory(drive.RootDirectory.FullName, searchTerm, results);

            if (results.Count >= 100)
                break;
        }

        return results.Take(100).ToArray();
    }

    private void SearchDirectory(string directory, string searchTerm, List<string> results)
    {
        if (results.Count >= 100)
            return;

        try
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                if (Path.GetFileName(file)
                    .Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(file);

                    if (results.Count >= 100)
                        return;
                }
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                if (results.Count >= 100)
                    return;

                SearchDirectory(
                    subDirectory,
                    searchTerm,
                    results);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip folders Windows doesn't allow us to access.
        }
        catch (IOException)
        {
            // Skip unavailable drives/folders/files.
        }
    }
}