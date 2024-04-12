namespace Hramos.API.Options;

public sealed class AzureOpenAI
{
    public string ChatDeploymentName { get; set; }

    public string Endpoint { get; set; }

    public string Key { get; set; }

    public string Model { get; set; }

    public string EmbeddingDeploymentName { get; set; }
}
