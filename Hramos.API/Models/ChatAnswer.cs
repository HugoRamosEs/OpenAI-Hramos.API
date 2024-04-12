using Hramos.API.Extensions;
using Hramos.API.Options;
using Microsoft.SemanticKernel;

namespace Hramos.API.Models
{
    /// <summary>
    /// An implementation of the <see cref="IChatAnswer"/> interface.
    /// </summary>
    public class ChatAnswer : IChatAnswer
    {
        private readonly Kernel kernel;
        private readonly string pluginsDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatAnswer"/> class.
        /// </summary>
        /// <param name="kernel">A valid instance of a <see cref="Kernel"/>.</param>
        public ChatAnswer(Kernel kernel)
        {
            this.kernel = kernel;
            this.pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), Constants.PluginsDirectory);
        }

        /// <inheritdoc />
        public async Task<string> GetAnswerFromUserPrompt(string prompt)
        {
            var result = await kernel.InvokePromptAsync(prompt);
            string response = result.ToString();
            return response;
        }

        /// <inheritdoc />
        public async Task<string> GetTranslatedTextFromUserPrompt(string lang, string text)
        {
            var arguments = new KernelArguments
            {
                { "lang", lang },
                { "input", text }
            };

            var result = await kernel.InvokeAsync<string>("TranslatePlugin", "Translate", arguments);
            string response = result.ToString();
            return response;
        }
    }
}
