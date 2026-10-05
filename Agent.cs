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
        var tools = new[]
        {
            AIFunctionFactory.Create(_fileTools.SearchFiles),
            AIFunctionFactory.Create(_fileTools.ReadTextFile),
            AIFunctionFactory.Create(_fileTools.ReadPdf),
            AIFunctionFactory.Create(_fileTools.OpenFile)
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
                         Tools =
                            [
                                AIFunctionFactory.Create(_fileTools.SearchFiles),
                                AIFunctionFactory.Create(_fileTools.ReadTextFile),
                                AIFunctionFactory.Create(_fileTools.ReadPdf),
                                AIFunctionFactory.Create(_fileTools.OpenFile)
                            ]
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
}