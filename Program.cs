using Microsoft.Extensions.AI;
using OllamaSharp;
using Permissions;
using Tools;

IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"),"qwen3:8b");


chatClient = chatClient.AsBuilder().UseFunctionInvocation().Build();
var permissionManager = new PermissionManager("Core/Permissions/permissions.json");

var httpClient = new HttpClient();

var steamTools = new SteamTools(httpClient);

var agent = new Agent(chatClient, permissionManager,steamTools);



await agent.Run();