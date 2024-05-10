using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

namespace Hramos.API.Models;

/// <summary>
/// A request for save and get data from Qdrant.
/// </summary>
public class RequestQdrant
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
}

