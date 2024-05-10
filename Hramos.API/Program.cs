using System.Diagnostics;

using Microsoft.OpenApi.Models;

using Hramos.API.Extensions;
using Hramos.API.Options;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

/* Load Configuration */
if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

// Azure and Qdrant options.
builder.Services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>().Bind(builder.Configuration.GetSection($@"{nameof(AzureOpenAIOptions)}")).ValidateDataAnnotations();
builder.Services.AddOptionsWithValidateOnStart<QdrantOptions>().Bind(builder.Configuration.GetSection($@"{ nameof(QdrantOptions)}")).ValidateDataAnnotations();

// Services.
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen();

builder.Services.AddSemanticKernel()
                .AddCosineStringSimilarityComparer()
                .AddLocalChatHistoryProvider()
                .AddChatAnswer();

builder.Services.AddHttpClient()
                .AddQdrantMemoryStore()
                .AddSemanticTextMemory();

// Swagger configuration and Controllers.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(Constants.AppVersion, new OpenApiInfo { Title = Constants.AppTitle, Version = Constants.AppVersion });
    c.EnableAnnotations();
});

builder.Services.AddControllers();

// Build the app.
var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(Constants.SwaggerEndpoint, Constants.AppTitle);
    });
}

app.UseHttpsRedirection()
   .UseAuthorization()
   .UseStatusCodePages();

app.MapControllers();

app.Run();
