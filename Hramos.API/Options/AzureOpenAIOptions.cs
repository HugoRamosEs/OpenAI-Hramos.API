using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

using Microsoft.Extensions.Options;

namespace Hramos.API.Options;

/// <summary>
/// Configuration options for Azure OpenAI service connection.
/// </summary>
public class AzureOpenAIOptions : IOptions<AzureOpenAIOptions>
{
    /// <summary>
    /// Chat deployment name of the model deployed in Azure OpenAI Studio.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string ChatDeploymentName { get; set; }

    /// <summary>
    /// Endpoint of the resource deployed in Azure.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Endpoint { get; set; }

    /// <summary>
    /// Key of the Azure OpenAI resource deployed in Azure.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Key { get; set; }

    /// <summary>
    /// Model of gpt used.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Model { get; set; }

    /// <summary>
    /// Model of embedding used.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string EmbeddingDeploymentName { get; set; }

    /// <summary>
    /// Object to return the value.
    /// </summary>
    public AzureOpenAIOptions Value => this;
}
