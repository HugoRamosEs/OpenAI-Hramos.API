using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

using Hramos.API.Options;

namespace Hramos.API.Models;

/// <summary>
/// A chat request utilizing chat memory.
/// </summary>
public class RequestChatHistory
{
    /// <summary>
    /// Conversation id.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string indexerId { get; set; }

    /// <summary>
    /// The input to be processed.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Input { get; set; }

    /// <summary>
    /// The language to translate the answer.
    /// </summary>
    public string? Locale { get; set; } = Constants.DefaultLanguage;
}
