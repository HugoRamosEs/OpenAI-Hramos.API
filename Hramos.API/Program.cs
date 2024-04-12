using Hramos.API.Extensions;
using Hramos.API.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

builder.Services.AddOptionsWithValidateOnStart<AzureOpenAI>().Bind(builder.Configuration.GetSection($@"{nameof(AzureOpenAI)}")).ValidateDataAnnotations();

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddSemanticKernel()
                .AddSemanticKernelCosineStringSimilarityComparer()
                .AddChatAnswer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hramos.API", Version = "v1" });
    c.EnableAnnotations();
});

builder.Services.AddControllers();

Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), Constants.PluginsDirectory));

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hramos.API");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStatusCodePages();
app.MapControllers();

app.Run();
