using Hramos.API.Extensions;
using Hramos.API.Options;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

builder.Services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>().Bind(builder.Configuration.GetSection($@"{nameof(AzureOpenAIOptions)}")).ValidateDataAnnotations();

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddSemanticKernel()
                .AddSemanticKernelCosineStringSimilarityComparer()
                .AddChatAnswer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(Constants.AppVersion, new OpenApiInfo { Title = Constants.AppTitle, Version = Constants.AppVersion });
    c.EnableAnnotations();
});

builder.Services.AddControllers();

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(Constants.swaggerEndpoint, Constants.AppTitle);
    });
}

app.UseHttpsRedirection()
   .UseAuthorization()
   .UseStatusCodePages();

app.MapControllers();

app.Run();
