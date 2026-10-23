using JobyAI.Api.Tools;
using JobyAI.Api.Security.Permissions;
using System.Text.Json;

public sealed class SearchFilesTool : IAssistantTool
{
    private readonly PermissionManager _permissionManager;

    public SearchFilesTool(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public string Name => "SearchFiles";

    public string Description =>
        "Search for files in approved folders by filename.";

    public string InputSchemaJson => """
    {
      "type": "object",
      "properties": {
        "searchTerm": {
          "type": "string"
        },
        "directory": {
          "type": ["string", "null"]
        }
      },
      "required": ["searchTerm"],
      "additionalProperties": false
    }
    """;

    public Task<ToolResult> ExecuteAsync(JsonElement arguments, ToolExecutionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // 1. Validate the arguments provided by the AI.
        if (arguments.ValueKind != JsonValueKind.Object || !arguments.TryGetProperty("searchTerm", out var searchTermElement) || searchTermElement.ValueKind != JsonValueKind.String)
        {
            return Task.FromResult(ToolResult.Failure("InvalidArguments", "A search term is required."));
        }

        var searchTerm = searchTermElement.GetString();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Task.FromResult(ToolResult.Failure("InvalidArguments", "The search term cannot be empty."));
        }

        // 2. Read the optional directory.
        string? directory = null;

        if (arguments.TryGetProperty("directory", out var directoryElement))
        {
            if (directoryElement.ValueKind == JsonValueKind.String)
            {
                directory = directoryElement.GetString();
            }
            else if (directoryElement.ValueKind != JsonValueKind.Null)
            {
                return Task.FromResult(ToolResult.Failure("InvalidArguments", "Directory must be a string or null."));
            }
        }

        // 3. Run your existing search logic.
        var results = new List<string>();
        var filesSearched = 0;

        if (string.IsNullOrWhiteSpace(directory))
        {
            // Search the user folders you have configured.
            foreach (var userDirectory in GetUserRelevantDirectories())
            {
                cancellationToken.ThrowIfCancellationRequested();

                SearchDirectory(userDirectory, searchTerm, results, ref filesSearched);

                if (results.Count >= 100)
                    break;
            }
        }
        else
        {
            if (!Directory.Exists(directory))
            {
                return Task.FromResult(
                    ToolResult.Failure(
                        "DirectoryNotFound",
                        $"Directory does not exist: {directory}"));
            }

            SearchDirectory(directory, searchTerm, results, ref filesSearched);
        }

        // 4. Return a structured result instead of a string.
        var resultMessage = results.Count == 0
            ? $"No files matching '{searchTerm}' were found."
            : $"Found {results.Count} files matching '{searchTerm}'.";

        return Task.FromResult(
            ToolResult.Success(
                resultMessage,
                new
                {
                    searchTerm,
                    directory,
                    filesSearched,
                    totalFound = results.Count,
                    files = results
                }));
    }


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
}