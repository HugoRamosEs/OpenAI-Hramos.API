using Hramos.API.Extensions;
using Microsoft.SemanticKernel;

namespace Hramos.API.Models
{
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
    }
}
