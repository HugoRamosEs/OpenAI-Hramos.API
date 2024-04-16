using Encamina.Enmarcha.Core.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Hramos.API.Models;

public sealed class RequestTranslate
{
    /// <summary>
    /// Gets or sets the language to translate the text to.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Lang { get; set; }

    /// <summary>
    /// Gets or sets the text to translate.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Text { get; set; }
}
