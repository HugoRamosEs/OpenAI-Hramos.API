using System.Text;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using Hramos.API.Abstractions;

namespace Hramos.API.Internals;

/// <summary>
/// An implementation of the <see cref="IChatAnswer"/> interface.
/// </summary>
public class ChatAnswer : IChatAnswer
{
    private readonly Kernel kernel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatAnswer"/> class.
    /// </summary>
    /// <param name="kernel">A valid instance of a <see cref="Kernel"/>.</param>
    public ChatAnswer(Kernel kernel)
    {
        this.kernel = kernel;
    }

    /// <inheritdoc />
    public async Task<string> GetAnswerFromUserPromptAsync(string prompt, CancellationToken cancellationToken)
    {
        var task = Task.Run(async () =>
        {
            return await kernel.InvokePromptAsync(prompt);
        }, cancellationToken);

        var result = await task;
        return result.ToString();
    }

    /// <inheritdoc />
    public async Task<string> GetTranslatedTextFromUserPromptAsync(string lang, string input, CancellationToken cancellationToken)
    {
        var arguments = new KernelArguments
        {
            { "lang", lang },
            { "input", input }
        };

        var result = await kernel.InvokeAsync<string>("TranslatePlugin", "Translate", arguments, cancellationToken);
        return result.ToString();
    }

    /// <inheritdoc />
    public async Task<string> GetAnswerFromUserWithSavedInfoAsync(string context, string input, string? locale, CancellationToken cancellationToken)
    {
        var arguments = new KernelArguments
        {
            { "context", context},
            { "input", input },
            { "locale", locale }
        };

        var result = await kernel.InvokeAsync<string>("DatabasePlugin", "AnswerWithStoredInfo", arguments, cancellationToken);
        return result.ToString();
    }

    /// <inheritdoc />
    public async Task<string> GetAnswerWithChatHistoryAsync(ChatHistory chatHistory, string input, string? locale, CancellationToken cancellationToken)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var message in chatHistory)
        {
            string role = message.Role.ToString();
            string content = message.Content.ToString();
            sb.AppendLine($"{role}: {content}" + Environment.NewLine);
        }
        var chatHistoryMessages = sb.ToString();

        Console.WriteLine(chatHistoryMessages);

        var arguments = new KernelArguments
        {
            { "chatHistory", chatHistoryMessages},
            { "input", input },
            { "locale", locale }
        };

        var result = await kernel.InvokeAsync<string>("DatabasePlugin", "AnswerWithChatHistory", arguments, cancellationToken);
        return result.ToString();
    }
}
