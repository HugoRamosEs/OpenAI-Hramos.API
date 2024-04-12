using Encamina.Enmarcha.AI.Abstractions;
using Hramos.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet("GetSemanticKernelCosine")]
        [SwaggerOperation(Summary = "Calculate the cosine similarity between two strings",
                          Description = "Returns the cosine similarity between two strings.")]
        public async Task<IActionResult> Get(string str1, string str2, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(str1) || string.IsNullOrWhiteSpace(str2))
            {
                return BadRequest("Both strings must be provided.");
            }

            try
            {
                var result = await stringSimilarityComparer.CompareAsync(str1, str2, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpPost("GetAnswerFromEndpoint")]
        [SwaggerOperation(Summary = "Get response from user prompt",
                          Description = "Returns the response from the user prompt.")]
        public async Task<IActionResult> Post(string str1)
        {
            if (string.IsNullOrWhiteSpace(str1))
            {
                return BadRequest("String must be provided.");
            }

            try
            {
                var result = await chatAnswer.GetAnswerFromUserPrompt(str1);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpPost("GetTranslatedTextFromUserPrompt")]
        [SwaggerOperation(Summary = "Get translated text from user prompt",
                          Description = "Returns the translated text from the user prompt.")]
        public async Task<IActionResult> Post(string lang, string text)
        {
            if (string.IsNullOrWhiteSpace(lang) || string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Both strings must be provided.");
            }

            try
            {
                var result = await chatAnswer.GetTranslatedTextFromUserPrompt(lang, text);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
    }
}
