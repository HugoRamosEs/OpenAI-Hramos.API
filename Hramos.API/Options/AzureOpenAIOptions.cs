using Microsoft.Extensions.Options;

namespace Hramos.API.Options
{
    public class AzureOpenAIOptions : IOptions<AzureOpenAIOptions>
    {
        public string ChatDeploymentName { get; set; }

        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string Model { get; set; }

        public string EmbeddingDeploymentName { get; set; }

        public AzureOpenAIOptions Value => this;
    }
}
