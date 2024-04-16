using Encamina.Enmarcha.Core.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Hramos.API.Models;

public class RequestCosine
{
    /// <summary>
    /// String 1 to compare.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Str1 { get; set; }

    /// <summary>
    /// String 2 to compare.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Str2 { get; set; }
}
