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

    public Agent(IChatClient chatClient, IChatClient visionClient, PermissionManager permissionManager, SteamTools steamTools, ScreenTools screenTools)
    {
        _chatClient = chatClient;
        _visionClient = visionClient;
        _permissionManager = permissionManager;
        _steamTools = steamTools;
        _screenTools = screenTools;
    }

    public async Task Run()
    {
        await TestVision();
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