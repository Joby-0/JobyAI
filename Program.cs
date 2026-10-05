using Microsoft.Extensions.AI;
using OllamaSharp;
using Permissions;
using Tools;

IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "qwen3:8b");
// IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"),"qwen3:14b");
var visionHttpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(5)
};

IChatClient visionClient = new OllamaApiClient(visionHttpClient,"qwen2.5vl-vision");



chatClient = chatClient.AsBuilder().UseFunctionInvocation().Build();
var permissionManager = new PermissionManager("Core/Permissions/permissions.json");

var httpClient = new HttpClient();

var steamTools = new SteamTools(httpClient);

var screenTools = new ScreenTools();

var fileTools = new FileTools(permissionManager);

var applicationTools = new ApplicationTools(permissionManager);

var agent = new Agent(chatClient, visionClient, permissionManager, steamTools, screenTools, fileTools, applicationTools);



await agent.Run();