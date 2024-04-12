using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.Core.DataAnnotations;
using Hramos.API.Extensions;
using Hramos.API.Models;
using Hramos.API.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Hramos.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SemanticKernelController : ControllerBase
    {
        private readonly IStringSimilarityComparer stringSimilarityComparer;
        private readonly IChatAnswer chatAnswer;

        public SemanticKernelController(IStringSimilarityComparer stringSimilarityComparer, IChatAnswer chatAnswer, IOptions<AzureOpenAI> options)
        {
            this.stringSimilarityComparer = stringSimilarityComparer;
            this.chatAnswer = chatAnswer;
        }

        [HttpGet("Cosine")]
        [SwaggerOperation(Summary = "Calculate the cosine similarity between two strings",
                          Description = "Returns the cosine similarity between two strings.")]
        public async Task<IActionResult> Get([NotEmptyOrWhitespace] string str1, [NotEmptyOrWhitespace]  string str2, CancellationToken cancellationToken)
        {
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

        [HttpPost("Chat")]
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

        [HttpPost("Translate")]
        [SwaggerOperation(Summary = "Get translated text from user prompt",
                          Description = "Returns the translated text from the user prompt.")]
        public async Task<IActionResult> Post(RequestTranslate request, CancellationToken cancellationToken)
        {

            var result = await chatAnswer.GetTranslatedTextFromUserPromptAsync(request.Lang, request.Text);

                throw new Exception("This is a test exception");

                return Ok(result);
            
        }
    }
}
