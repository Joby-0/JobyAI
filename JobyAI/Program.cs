using Microsoft.Extensions.AI;
using OllamaSharp;

Console.WriteLine("Hello, World!");

IChatClient chatClient = new OllamaApiClient("http://localhost:11434", "qwen3:8b");

var response = await chatClient.GetResponseAsync("what can u do?");

Console.WriteLine(response.Text);
