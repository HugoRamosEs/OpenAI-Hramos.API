using Encamina.Enmarcha.Core;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;

using Hramos.API.Abstractions;
using Hramos.API.Options;
using Hramos.API.Internals;

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

        #pragma warning disable SKEXP0001
        #pragma warning disable SKEXP0020
        /// <summary>
        /// Adds Qdrant vector database as a memory store (<see cref="IMemoryStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
        /// </summary>
        /// <remarks>
        /// This extension methods requires a <see cref="QdrantOptions"/> to be already configured.
        /// </remarks>
        /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddQdrantMemoryStore(this IServiceCollection services)
        {
            return services.AddSingleton<IMemoryStore>(sp =>
            {
                var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<QdrantOptions>>();

                var httpClient = new HttpClient(new HttpClientHandler()
                {
                    CheckCertificateRevocationList = true,
                }, disposeHandler: false);

                static QdrantMemoryStore Builder(IServiceProvider serviceProvider, HttpClient httpClient, QdrantOptions options)
                {
                    httpClient.ConfigureHttpClientForQdrant(options);

                    return new QdrantMemoryStore(httpClient, options.VectorSize, loggerFactory: serviceProvider.GetService<ILoggerFactory>());
                }

                var debouncedBuilder = Debouncer.Debounce<QdrantOptions>(options => Builder(sp, httpClient, options), 300);

                var memory = Builder(sp, httpClient, optionsMonitor.CurrentValue);

                optionsMonitor.OnChange(debouncedBuilder);

                return memory;
            });
        }

        /// <summary>
        /// Adds semantic text memory (<see cref="ISemanticTextMemory"/>) to the <see cref="IServiceCollection"/> in the <see cref="ServiceLifetime.Scoped">scoped service lifetime</see>,
        /// based on user preferences.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddSemanticTextMemory(this IServiceCollection services)
        {
            return services.AddScoped(sp =>
            {
                var azureOpenAIOptions = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;
                var memoryStore = sp.GetRequiredService<IMemoryStore>();

                return new MemoryBuilder()
                    .WithAzureOpenAITextEmbeddingGeneration(azureOpenAIOptions.EmbeddingsModel, azureOpenAIOptions.Endpoint, azureOpenAIOptions.Key)
                    .WithMemoryStore(sp.GetRequiredService<IMemoryStore>())
                    .Build();
            });
        }
        #pragma warning restore SKEXP0001
        #pragma warning restore SKEXP0020

        public static IServiceCollection AddQdrantSnapshotHandler(this IServiceCollection services)
        {
            return services.AddSingleton<IQdrantSnapshotHandler, QdrantSnapshotHandler>();
        }
    }
}
