using Core;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"),"qwen3:8b");


chatClient = chatClient
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();
var agent = new Agent(chatClient);



await agent.Run();