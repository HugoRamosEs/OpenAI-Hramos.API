using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.Core.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using Hramos.API.Abstractions;
using Hramos.API.Models;

namespace Hramos.API.Controllers;

/// <summary>
/// Controller for the Semantic Kernel. It contains the endpoints for the Cosine similarity, Chat answer, and Translate text.
/// </summary>
[ApiController]
[Route("[controller]")]
public class SemanticKernelController : ControllerBase
{
    private readonly IStringSimilarityComparer stringSimilarityComparer;
    private readonly IChatAnswer chatAnswer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticKernelController"/> class.
    /// </summary>
    /// <param name="stringSimilarityComparer">A valid <see cref="IStringSimilarityComparer"/> instance.</param>
    /// <param name="chatAnswer">A valid <see cref="IChatAnswer"/> instance.</param>
    public SemanticKernelController(IStringSimilarityComparer stringSimilarityComparer, IChatAnswer chatAnswer)
    {
        this.stringSimilarityComparer = stringSimilarityComparer;
        this.chatAnswer = chatAnswer;
    }

    /// <summary>
    /// Calculates the cosine similarity between two strings.
    /// </summary>
    /// <param name="request">A valid instance of <see cref="RequestCosine"/></param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>Cosine similarity value between the strings.</returns>
    [HttpPost("Cosine")]
    [SwaggerOperation(Summary = "Calculate the cosine similarity between two strings",
                      Description = "Returns the cosine similarity between two strings.")]
    public async Task<IActionResult> Post(RequestCosine request, CancellationToken cancellationToken)
    {
        var result = await stringSimilarityComparer.CompareAsync(request.Str1, request.Str2, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the response from the user prompt.
    /// </summary>
    /// <param name="str1">The user prompt string.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The response from the user prompt.</returns>
    [HttpPost("Chat")]
    [SwaggerOperation(Summary = "Get response from user prompt",
                      Description = "Returns the response from the user prompt.")]
    public async Task<IActionResult> Post([NotEmptyOrWhitespace] string str1, CancellationToken cancellationToken)
    {
        var result = await chatAnswer.GetAnswerFromUserPromptAsync(str1, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the translated text from the user prompt.
    /// </summary>
    /// <param name="request">A valid instance of <see cref="RequestTranslate"/></param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns></returns>
    [HttpPost("Translate")]
    [SwaggerOperation(Summary = "Get translated text from user prompt",
                      Description = "Returns the translated text from the user prompt.")]
    public async Task<IActionResult> Post(RequestTranslate request, CancellationToken cancellationToken)
    {
        var result = await chatAnswer.GetTranslatedTextFromUserPromptAsync(request.Lang, request.Input, cancellationToken);
        return Ok(result);
    }
}
