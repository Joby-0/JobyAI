using System.ComponentModel;
using System.Diagnostics;
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
       "Searches for files on the computer. " +
       "If the user specifies a directory, search only that directory. " +
       "If the user does not specify a directory, leave directory empty " +
       "and search the entire computer."
   )]
    public string SearchFiles(
       [Description(
        "The filename text to search for. " +
        "Use the main filename subject, for example 'CV'."
    )]
    string searchTerm,

       [Description(
        "Optional directory to search in. " +
        "Leave empty when no directory was specified by the user."
    )]
    string? directory = null)
    {
        Console.WriteLine();
        Console.WriteLine(">>> SEARCHFILES TOOL WAS CALLED <<<");
        Console.WriteLine($"Searching for: {searchTerm}");
        Console.WriteLine($"Directory: {directory}");

        var results = new List<string>();
        var filesearched = 0;

        if (string.IsNullOrWhiteSpace(directory))
        {
            // No directory specified → search entire computer
            foreach (var userDirectory in GetUserRelevantDirectories())
            {
                SearchDirectory(
                    userDirectory,
                    searchTerm,
                    results,
                    filesearched);

                if (results.Count >= 100)
                    break;
            }
            
        }
        else if (results.Count != 0)
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady)
                    continue;

                SearchDirectory(
                    drive.RootDirectory.FullName,
                    searchTerm,
                    results,
                    filesearched);

                if (results.Count >= 100)
                    break;
            }
        }
        else
        {
            if (!Directory.Exists(directory))
                return $"Directory does not exist: {directory}";

            SearchDirectory(
                directory,
                searchTerm,
                results,
                filesearched);
        }

        if (results.Count == 0)
        {
            return $"No files matching '{searchTerm}' were found.";
        }

        var output = new StringBuilder();

        output.AppendLine(
            $"Found {results.Count} files matching '{searchTerm}':");

        for (int i = 0; i < results.Count; i++)
        {
            output.AppendLine($"{i + 1}. {results[i]}");
        }

        return output.ToString();
    }

    private void SearchDirectory(string directory, string searchTerm, List<string> results, int filesearched)
    {
        // Limit the number of results to 100 to avoid overwhelming the user.
        if (results.Count >= 100)
            return;

        if (ShouldSkipDirectory(directory))
            return;

        var permission = _permissionManager.GetAccessLevel(directory);

        // Console.WriteLine($"Checking: {directory} | Permission: {permission}");
        Console.WriteLine($"\r Searching: {directory}... Files searched: {filesearched} | Results found: {results.Count}");
        if (permission < PermissionLevel.Read)
            return;



        try
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                filesearched++;
                if (Path.GetFileName(file).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(file);

                    if (results.Count >= 100)
                        return;
                }
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                SearchDirectory(subDirectory, searchTerm, results, filesearched);

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

    private bool ShouldSkipDirectory(string directory)
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

    private IEnumerable<string> GetUserRelevantDirectories()
    {
        var directories = new[]
        {
        Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments),

        Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop),

        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile),
            "Downloads"),

        Environment.GetFolderPath(
            Environment.SpecialFolder.MyPictures),

        Environment.GetFolderPath(
            Environment.SpecialFolder.MyVideos),

        Environment.GetFolderPath(
            Environment.SpecialFolder.MyMusic)
    };

        return directories
            .Where(Directory.Exists)
            .Distinct(StringComparer.OrdinalIgnoreCase);
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

    [Description(
        "Opens a file using the default Windows application associated with that file type. " +
        "Use this when the user asks to open, launch, or show a file. " +
        "The file must already exist. Never invent a file path."
    )]
    public string OpenFile(
        [Description(
            "The full path of the file to open. " +
            "Example: C:\\Users\\JohnDoe\\Documents\\CV.pdf"
        )]
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return "No file path was provided.";

        if (!File.Exists(filePath))
            return $"File does not exist: {filePath}";

        var directory = Path.GetDirectoryName(filePath);

        if (directory is null)
            return "Could not determine the file directory.";

        var permission = _permissionManager.GetAccessLevel(filePath);

        if (permission == PermissionLevel.None)
        {
            permission = _permissionManager.AskForAccess(directory);
        }

        if (permission is PermissionLevel.None or PermissionLevel.Denied)
            return $"I don't have permission to open: {filePath}";

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });

            return $"Opened file: {filePath}";
        }
        catch (Exception ex)
        {
            return $"Could not open file: {ex.Message}";
        }
    }
}
