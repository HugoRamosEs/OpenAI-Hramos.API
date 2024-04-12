using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

using Hramos.API.Models.Objects;
using Hramos.API.Models;
using Hramos.API.Options;

namespace Hramos.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
        {
            // GPT-4
            //var azureOpenAIOptions = new AzureOpenAI
            //{
            //    ChatDeploymentName = "hramos-gpt-4-1106prev",
            //    Endpoint = "https://hramos-azure-openai.openai.azure.com/",
            //    Key = "355d1f33242545acb46e910556d0c9ab",
            //    Model = "gpt-4",
            //    EmbeddingDeploymentName = "hramos-ada-002"
            //};

            // GPT-3.5-TURBO
            var azureOpenAIOptions = new AzureOpenAI
            {
                ChatDeploymentName = "hramos-gpt-35-turbo-16k",
                Endpoint = "https://hramos-azure-openai.openai.azure.com/",
                Key = "355d1f33242545acb46e910556d0c9ab",
                Model = "gpt-35-turbo-16",
                EmbeddingDeploymentName = "hramos-ada-002"
            };

            services.AddScoped(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<Kernel>>();
                // var options = sp.GetRequiredService<IOptions<SemanticKernelOptions>>().Value;

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
