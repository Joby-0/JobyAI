using Microsoft.Extensions.AI;
using Permissions;
using Tools;

public class Agent
{
    private readonly IChatClient _chatClient;
    private readonly IChatClient _visionClient;
    private readonly FileTools _fileTools;
    private readonly ApplicationTools _applicationTools;
    private readonly PermissionManager _permissionManager;
    private readonly SteamTools _steamTools;

    private readonly ScreenTools _screenTools;

    public Agent(IChatClient chatClient, IChatClient visionClient, PermissionManager permissionManager, SteamTools steamTools, ScreenTools screenTools, FileTools fileTools, ApplicationTools applicationTools)
    {
        _chatClient = chatClient;
        _visionClient = visionClient;
        _permissionManager = permissionManager;
        _steamTools = steamTools;
        _screenTools = screenTools;
        _fileTools = fileTools;
        _applicationTools = applicationTools;
    }

    public async Task Run()
    {
        var tools = new[]
        {
            AIFunctionFactory.Create(_fileTools.SearchFiles),
            AIFunctionFactory.Create(_fileTools.ReadTextFile),
            AIFunctionFactory.Create(_fileTools.ReadPdf),
            AIFunctionFactory.Create(_fileTools.OpenFile),

            AIFunctionFactory.Create(_applicationTools.SearchApplications),
            AIFunctionFactory.Create(_applicationTools.OpenApplication),

            AIFunctionFactory.Create(_steamTools.SearchSteamGames),
            AIFunctionFactory.Create(_steamTools.OpenSteamGame),

            AIFunctionFactory.Create(_screenTools.TakeScreenshot)
        };

        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                ChatRole.System,
                """
                You are a local computer agent.

                You can use tools multiple times.

                A tool returning no results does NOT mean that the user's requested
                file does not exist.

                If a search returns zero results, reconsider your search query and
                try a broader or different search before giving up.

                For example:

                User: "Find CV 5"

                If searching for "CV 5" returns no results, try searching for "CV".

                After receiving search results, inspect them and decide which result
                best matches the user's request.

                Do not give up after the first failed search.
                """)
        };

        while (true)
        {
            Console.Write("> ");

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            messages.Add(new ChatMessage(ChatRole.User, input));

            while (true)
            {
                var response = await _chatClient.GetResponseAsync(
                    messages,
                     new ChatOptions
                     {
                         Tools = tools
                     });

                messages.AddRange(response.Messages);
                Console.WriteLine(response);
                // If the model has finished reasoning/tool usage,
                // print the final response and return to the user.
                if (!response.Messages.Any(message => message.Contents.OfType<FunctionCallContent>().Any()))
                {
                    Console.WriteLine(response.Text);
                    break;
                }
            }
        }
    }


    private async Task TestVision()
    {
        var screenshotResult = _screenTools.TakeScreenshot();

        Console.WriteLine(screenshotResult);

        var screenshotPath = Path.Combine(
            Path.GetTempPath(),
            "agent-screen.png");

        if (!File.Exists(screenshotPath))
        {
            Console.WriteLine("Screenshot was not created.");
            return;
        }

        var imageBytes = await File.ReadAllBytesAsync(screenshotPath);

        var message = new ChatMessage(
            ChatRole.User,
            [
                new TextContent(
                "Look at this screenshot and describe what is currently visible on the screen."
            ),
            new DataContent(
                imageBytes,
                "image/png")
            ]);

        var response = await _visionClient.GetResponseAsync(
            [message]);

        Console.WriteLine();
        Console.WriteLine("VISION:");
        Console.WriteLine(response.Text);
    }
}