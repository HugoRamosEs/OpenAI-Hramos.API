﻿using Microsoft.SemanticKernel;

namespace Hramos.API.Extensions
{
    public interface IChatAnswer
    {
        /// <summary>
        /// Generate an answer using an AI model, via the user's prompt.
        /// </summary>
        /// <param name="prompt">Prompt received from the user.</param>
        /// <returns>Answer generated by the AI model.</returns>
        Task<string> GetAnswerFromUserPromptAsync(string prompt, CancellationToken cancellationToken);

        /// <summary>
        /// Get a summary from the user prompt.
        /// </summary>
        /// <param name="lang">Language to translate the text to.</param>
        /// <param name="input">Text to translate.</param>
        /// <returns>The translated text.</returns>
        Task<string> GetTranslatedTextFromUserPromptAsync(string lang, string input, CancellationToken cancellationToken);
    }
}
