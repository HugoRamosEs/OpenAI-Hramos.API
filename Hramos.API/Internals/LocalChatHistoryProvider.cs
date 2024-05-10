using Encamina.Enmarcha.SemanticKernel.Abstractions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Hramos.API.Internals;

/// <summary>
/// An implementation of the <see cref="IChatHistoryProvider"/> interface.
/// </summary>
public class LocalChatHistoryProvider : IChatHistoryProvider
{
    public Dictionary<string, List<ChatMessageContent>> ChatMessagesByIndexerId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalChatHistoryProvider"/> class.
    /// </summary>
    public LocalChatHistoryProvider()
    {
        ChatMessagesByIndexerId = new Dictionary<string, List<ChatMessageContent>>();
    }

    /// <summary>
    /// Deletes all chat history messages for a specific user.
    /// </summary>
    /// <param name="indexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    public async Task DeleteChatMessagesHistoryAsync(string indexerId, CancellationToken cancellationToken)
    {
        if (ChatMessagesByIndexerId.ContainsKey(indexerId))
        {            
            ChatMessagesByIndexerId.Remove(indexerId);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Loads chat history messages.
    /// </summary>
    /// <param name="chatHistory">The current chat history.</param>
    /// <param name="indexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="remainingTokens">The total remaining tokens available for loading messages from the chat history.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    public async Task LoadChatMessagesHistoryAsync(ChatHistory chatHistory, string indexerId, int remainingTokens, CancellationToken cancellationToken)
    {
        if (ChatMessagesByIndexerId.ContainsKey(indexerId))
        {
           chatHistory.AddRange(ChatMessagesByIndexerId[indexerId]);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Saves a chat message into the conversation history.
    /// </summary>
    /// <param name="indexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="roleName">The name of the role associated with the chat message. For example the `user`, the `assistant` or the `system`.</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    public async Task SaveChatMessagesHistoryAsync(string indexerId, string roleName, string message, CancellationToken cancellationToken)
    {
        ChatMessageContent chatMessageContent = new ChatMessageContent()
        {
            Role = roleName == AuthorRole.User.ToString() ? AuthorRole.User : AuthorRole.Assistant,
            Content = message
        };

        if (ChatMessagesByIndexerId.ContainsKey(indexerId))
        {
            ChatMessagesByIndexerId[indexerId].Add(chatMessageContent);
        }
        else
        {
            ChatMessagesByIndexerId.Add(indexerId, new List<ChatMessageContent>() { chatMessageContent });
        }

        await Task.CompletedTask;
    }
}
