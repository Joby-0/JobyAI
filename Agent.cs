using Microsoft.Extensions.AI;
using Permissions;
using Tools;

public class Agent
{
    private readonly IChatClient _chatClient;
    private readonly FileTools _fileTools;


    public Agent(IChatClient chatClient, PermissionManager permissionManager)
    {
        _chatClient = chatClient;
        _fileTools = new FileTools(permissionManager);
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
                        AIFunctionFactory.Create(_fileTools.SearchFiles),
                        AIFunctionFactory.Create(_fileTools.ReadTextFile),
                        AIFunctionFactory.Create(_fileTools.ReadPdf)
                    ]
                });


            Console.WriteLine(response.Text);
        }
    }
}