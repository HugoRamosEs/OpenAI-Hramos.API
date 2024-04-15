using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.Core.DataAnnotations;
using Hramos.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Hramos.API.Models.Objects;

namespace Hramos.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SemanticKernelController : ControllerBase
    {
        private readonly IStringSimilarityComparer stringSimilarityComparer;
        private readonly IChatAnswer chatAnswer;

        public SemanticKernelController(IStringSimilarityComparer stringSimilarityComparer, IChatAnswer chatAnswer)
        {
            this.stringSimilarityComparer = stringSimilarityComparer;
            this.chatAnswer = chatAnswer;
        }

        [HttpPost("Cosine")]
        [SwaggerOperation(Summary = "Calculate the cosine similarity between two strings",
                          Description = "Returns the cosine similarity between two strings.")]
        public async Task<IActionResult> Post(RequestCosine request, CancellationToken cancellationToken)
        {
            var result = await stringSimilarityComparer.CompareAsync(request.Str1, request.Str2, cancellationToken);
            return Ok(result);
        }

        [HttpPost("Chat")]
        [SwaggerOperation(Summary = "Get response from user prompt",
                          Description = "Returns the response from the user prompt.")]
        public async Task<IActionResult> Post([NotEmptyOrWhitespace] string str1, CancellationToken cancellationToken)
        {
            var result = await chatAnswer.GetAnswerFromUserPromptAsync(str1, cancellationToken);
            return Ok(result);
        }

        [HttpPost("Translate")]
        [SwaggerOperation(Summary = "Get translated text from user prompt",
                          Description = "Returns the translated text from the user prompt.")]
        public async Task<IActionResult> Post(RequestTranslate request, CancellationToken cancellationToken)
        {
            var result = await chatAnswer.GetTranslatedTextFromUserPromptAsync(request.Lang, request.Text, cancellationToken);
            return Ok(result);
        }
    }
}
