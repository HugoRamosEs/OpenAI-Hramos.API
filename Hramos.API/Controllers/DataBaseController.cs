#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using System.Text;

using Encamina.Enmarcha.SemanticKernel.Abstractions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.ChatCompletion;

using Swashbuckle.AspNetCore.Annotations;

using Hramos.API.Abstractions;
using Hramos.API.Models;

namespace Hramos.API.Controllers;

/// <summary>
/// Controller for interacting with semantic text memory, chat history, and Qdrant-based search.
/// </summary>
[ApiController]
[Route("[controller]")]
public class DataBaseController : ControllerBase
{
    private readonly ISemanticTextMemory semanticTextMemory;
    private readonly IChatAnswer chatAnswer;
    private readonly IChatHistoryProvider chatHistoryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBaseController"/> class.
    /// </summary>
    /// <param name="semanticTextMemory">A valid <see cref="ISemanticTextMemory"/> instance.</param>
    /// <param name="chatAnswer">A valid <see cref="IChatAnswer"/> instance.</param>
    /// <param name="chatHistoryProvider">A valid <see cref="IChatHistoryProvider"/> instance.</param>
    public DataBaseController(ISemanticTextMemory semanticTextMemory, IChatAnswer chatAnswer, IChatHistoryProvider chatHistoryProvider)
    {
        this.semanticTextMemory = semanticTextMemory;
        this.chatAnswer = chatAnswer;
        this.chatHistoryProvider = chatHistoryProvider;
    }


    /// <summary>
    /// Stores data in the Qdrant database.
    /// </summary>
    /// <param name="request">A valid <see cref="RequestQdrant"/>instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A successful response with the database save result.</returns>
    [HttpPost("Save")]
    [SwaggerOperation(Summary = "Save information in the database",
                      Description = "Saves information in the database.")]
    public async Task<IActionResult> SaveInformation(RequestQdrant request, CancellationToken cancellationToken)
    {
        var result = await semanticTextMemory.SaveInformationAsync(request.CollectionName, request.Input, Guid.NewGuid().ToString(), cancellationToken: cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the most relevant information from the Qdrant database based on the user's query.
    /// </summary>
    /// <param name="request">A valid <see cref="RequestQdrant"/>instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A successful response with the database search result.</returns>
    [HttpPost("Search")]
    [SwaggerOperation(Summary = "Search for an answer for the user in Qdrant",
                      Description = "Find the closest value in Qdrant, to return a response to the user prompt.")]
    public async Task<IActionResult> Search(RequestQdrant request, CancellationToken cancellationToken)
    {
        var result = await semanticTextMemory.SearchAsync(request.CollectionName, request.Input, 5, cancellationToken: cancellationToken)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a response based on the user prompt using the stored information in Qdrant.
    /// </summary>
    /// <param name="request">A valid <see cref="RequestChatQdrant"/>instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A successful response containing the answer to the user's prompt</returns>
    [HttpPost("Chat")]
    [SwaggerOperation(Summary = "Gets a response with stored information.",
                      Description = "Obtains a response with information stored in a database.")]
    public async Task<IActionResult> ChatAnswerWithStoredInfo(RequestChatQdrant request, CancellationToken cancellationToken)
    {
        var memoryQueryResults = await semanticTextMemory.SearchAsync(request.CollectionName, request.Input, 5, cancellationToken: cancellationToken)
                                .ToListAsync(cancellationToken: cancellationToken);

        StringBuilder sb = new StringBuilder();
        foreach (var item in memoryQueryResults)
        {
            sb.AppendLine(item.Metadata.Text + Environment.NewLine + Environment.NewLine);
        }
        var context = sb.ToString();

        var result = await chatAnswer.GetAnswerFromUserWithSavedInfoAsync(context, request.Input, request.Locale, cancellationToken: cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a response from the user's prompt along with chat history.
    /// </summary>
    /// <param name="request">A valid <see cref="RequestChatHistory"/>instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A successful response containing the answer to the user's prompt</returns>
    [HttpPost("ChatHistory")]
    [SwaggerOperation(Summary = "Chat with history.",
                      Description = "Chat with an AI that has memory with chat history.")]
    public async Task<IActionResult> ChatWithMemory(RequestChatHistory request, CancellationToken cancellationToken)
    {
        var chatHistory = new ChatHistory();
        await chatHistoryProvider.LoadChatMessagesHistoryAsync(chatHistory, request.indexerId, 5, cancellationToken);

        var response = await chatAnswer.GetAnswerWithChatHistoryAsync(chatHistory, request.Input, request.Locale, cancellationToken);

        await chatHistoryProvider.SaveChatMessagesHistoryAsync(request.indexerId, AuthorRole.User.ToString(), request.Input, cancellationToken);
        await chatHistoryProvider.SaveChatMessagesHistoryAsync(request.indexerId, AuthorRole.Assistant.ToString(), response, cancellationToken);

        return Ok(response);
    }
}

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
