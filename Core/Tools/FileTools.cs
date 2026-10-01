using System.ComponentModel;
using System.Text;
using Permissions;
using UglyToad.PdfPig;

namespace Tools;

public class FileTools
{
    private readonly PermissionManager _permissionManager;

    public FileTools(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }
    
    [Description(
    "Searches a specific directory for files matching a filename search term. " +
    "Use this when the user asks to find, locate, search for, or look for a file. " +
    "If the user provides a directory, only search inside that directory. " +
    "Do not search the entire computer when a directory is provided."
)]
    public string SearchFiles(
    [Description(
        "The text to search for in file names. " +
        "Examples: CV, JobySaaS, invoice, photo, report"
    )]
    string searchTerm,

    [Description(
        "The directory to search in. " +
        "If the user provides a directory, use that directory. " +
        "Example: C:\\Users\\JohnDoe\\Documents"
    )]
    string directory)
    {
        Console.WriteLine();
        Console.WriteLine(">>> SEARCHFILES TOOL WAS CALLED <<<");
        Console.WriteLine($"Searching for: {searchTerm}");
        Console.WriteLine($"Directory: {directory}");
        Console.WriteLine();

        if (string.IsNullOrWhiteSpace(directory))
            return "No directory was provided.";

        if (!Directory.Exists(directory))
            return $"Directory does not exist: {directory}";

        var permission = _permissionManager.GetAccessLevel(directory);

        Console.WriteLine(
            $"Directory permission: {permission}");

        if (permission == PermissionLevel.None)
        {
            permission = _permissionManager.AskForAccess(directory);
        }

        if (permission < PermissionLevel.Read)
        {
            return $"I don't have permission to search: {directory}";
        }

        var results = new List<string>();

        SearchDirectory(
            directory,
            searchTerm,
            results);

        Console.WriteLine($"Found {results.Count} matching files.");
        var orderedResults = results
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Take(100)
            .ToList();

        if (orderedResults.Count == 0)
        {
            return $"No files matching '{searchTerm}' were found in {directory}.";
        }

        var output = new StringBuilder();

        output.AppendLine(
            $"Found {orderedResults.Count} matching files:");

        for (int i = 0; i < orderedResults.Count; i++)
        {
            output.AppendLine(
                $"{i + 1}. {orderedResults[i]}");
        }

        return output.ToString();
    }

    private void SearchDirectory(string directory, string searchTerm, List<string> results)
    {
        if (results.Count >= 100)
            return;

        var permission = _permissionManager.GetAccessLevel(directory);
        Console.WriteLine($"Checking: {directory} | Permission: {permission}");
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


    [Description("Reads the contents of a text file. " + "Use this whenever the user asks to read, open, or view a text-based file.")]
    public string ReadTextFile([Description("The full path of the file to read. " + "Example: C:\\Users\\JohnDoe\\Documents\\file.txt")] string filePath)
    {
        if (!File.Exists(filePath))
        {
            return $"File does not exist: {filePath}";
        }

        var permission = _permissionManager.GetAccessLevel(filePath);

        if (permission == PermissionLevel.None)
        {
            permission = _permissionManager.AskForAccess(Path.GetDirectoryName(filePath)!);
        }

        if (permission < PermissionLevel.Read)
        {
            return $"I don't have permission to read: {filePath}";
        }

        try
        {
            return File.ReadAllText(filePath);
        }
        catch (UnauthorizedAccessException)
        {
            return $"Windows denied access to: {filePath}";
        }
        catch (IOException ex)
        {
            return $"Could not read the file: {ex.Message}";
        }
    }

    [Description("Reads the contents of a PDF file. " + "Use this whenever the user asks to read, open, or view a PDF file.")]
    public string ReadPdf([Description("The full path of the PDF file to read. " + "Example: C:\\Users\\JohnDoe\\Documents\\file.pdf")] string filePath)
    {
        if (!File.Exists(filePath))
            return $"PDF file not found: {filePath}";

        var permission = _permissionManager.GetAccessLevel(filePath);

        if (permission == PermissionLevel.None)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (directory is null)
                return "Could not determine the file directory.";

            permission = _permissionManager.AskForAccess(directory);
        }

        if (permission < PermissionLevel.Read)
            return $"I don't have permission to read: {filePath}";

        try
        {
            using var document = PdfDocument.Open(filePath);

            var text = new System.Text.StringBuilder();

            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }

            return text.ToString();
        }
        catch (UnauthorizedAccessException)
        {
            return $"Windows denied access to: {filePath}";
        }
        catch (Exception ex)
        {
            return $"Could not read PDF: {ex.Message}";
        }
    }
}