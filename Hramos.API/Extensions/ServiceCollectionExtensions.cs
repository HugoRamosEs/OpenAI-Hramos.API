#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Encamina.Enmarcha.Core;
using Encamina.Enmarcha.SemanticKernel.Abstractions;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;

using Hramos.API.Abstractions;
using Hramos.API.Options;
using Hramos.API.Internals;

namespace Hramos.API.Extensions;

/// <summary>
/// Extension methods to configure default services for OpenAI-based and Qdrant services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a semantic kernel (<see cref="Kernel"/>) to the <see cref="IServiceCollection"/> as scoped service.
    /// </summary>
    /// <remarks>
    /// This extension method requires a <see cref="AzureOpenAIOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSemanticKernel(this IServiceCollection services)
    {
        services.TryAddScoped(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Kernel>>();
            var azureOpenAIOptions = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

            var kernelBuilder = Kernel.CreateBuilder()
                                      .AddAzureOpenAIChatCompletion(azureOpenAIOptions.ChatDeploymentName, azureOpenAIOptions.Endpoint, azureOpenAIOptions.Key)
                                      .AddAzureOpenAITextEmbeddingGeneration(azureOpenAIOptions.EmbeddingDeploymentName, azureOpenAIOptions.Endpoint, azureOpenAIOptions.Key);

            kernelBuilder.Services.AddSingleton(logger);
            var kernel = kernelBuilder.Build();

            // Imported plugins...
            kernel.ImportPluginFromPromptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PluginsDirectory, @"TranslatePlugin"), @"TranslatePlugin");
            kernel.ImportPluginFromPromptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PluginsDirectory, @"DatabasePlugin"), @"DatabasePlugin");

            return kernel;
        });

        return services;
    }

    /// <summary>
    /// Adds a chat answer from user prompt (<see cref="IChatAnswer"/>) to the <see cref="IServiceCollection"/> as scoped service.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddChatAnswer(this IServiceCollection services)
    {
        services.AddScoped<IChatAnswer, ChatAnswer>();

        return services;
    }

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
                .WithAzureOpenAITextEmbeddingGeneration(azureOpenAIOptions.EmbeddingDeploymentName, azureOpenAIOptions.Endpoint, azureOpenAIOptions.Key)
                .WithMemoryStore(memoryStore)
                .Build();
        });
    }

    /// <summary>
    /// Adds a local chat history provider (<see cref="IChatHistoryProvider"/>) to the <see cref="IServiceCollection"/> as scoped service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddLocalChatHistoryProvider(this IServiceCollection services)
    {
        services.AddSingleton<IChatHistoryProvider, LocalChatHistoryProvider>();

        return services;
    }
}

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
