using Microsoft.Extensions.AI;
using OllamaSharp;
using Permissions;

IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"),"qwen3:8b");


chatClient = chatClient.AsBuilder().UseFunctionInvocation().Build();
var permissionManager = new PermissionManager("Core/Permissions/permissions.json");

var agent = new Agent(chatClient, permissionManager);



await agent.Run();