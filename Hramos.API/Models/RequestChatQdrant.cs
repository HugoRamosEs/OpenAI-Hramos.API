using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

using Hramos.API.Options;

namespace Hramos.API.Models;

/// <summary>
/// A chat request utilizing information stored in Qdrant.
/// </summary>
public sealed class RequestChatQdrant
{
    /// <summary>
    /// The name of the collection to be used.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string CollectionName { get; set; }

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
