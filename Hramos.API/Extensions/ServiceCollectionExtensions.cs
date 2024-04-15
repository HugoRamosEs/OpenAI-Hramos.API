using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.SemanticKernel;
using Hramos.API.Models;
using Hramos.API.Options;
using Microsoft.Extensions.Options;

namespace Hramos.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
        {
            services.TryAddScoped(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<Kernel>>();
                var azureOpenAIOptions = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

                #pragma warning disable SKEXP0010
                var kernelBuilder = Kernel.CreateBuilder()
                                          .AddAzureOpenAIChatCompletion(azureOpenAIOptions.ChatDeploymentName, azureOpenAIOptions.Endpoint, azureOpenAIOptions.Key)
                                          .AddAzureOpenAITextEmbeddingGeneration(azureOpenAIOptions.EmbeddingDeploymentName, azureOpenAIOptions.Endpoint, azureOpenAIOptions.Key);
                #pragma warning restore SKEXP0010

                kernelBuilder.Services.AddSingleton(logger);
                var kernel = kernelBuilder.Build();

                kernel.ImportPluginFromPromptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PluginsDirectory, @"TranslatePlugin"), "TranslatePlugin");

                return kernel;
            });

            return services;
        }

        public static IServiceCollection AddChatAnswer(this IServiceCollection services)
        {
            services.AddScoped<IChatAnswer, ChatAnswer>();
            return services;
        }
    }
}
