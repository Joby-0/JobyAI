using Microsoft.Extensions.AI;
using Tools;

namespace Core;

public class Agent
{
    private readonly IChatClient _chatClient;
    private readonly FileTools _fileTools;

    public Agent(IChatClient chatClient)
    {
        _chatClient = chatClient;
        _fileTools = new FileTools();
    }

    public async Task Run()
    {
        while (true)
        {
            Console.Write("> ");

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            var response = await _chatClient.GetResponseAsync(
                input,
                new ChatOptions
                {
                    Tools =
                    [
                        AIFunctionFactory.Create(_fileTools.SearchFiles)
                    ]
                });


            Console.WriteLine(response.Text);
        }
    }
}