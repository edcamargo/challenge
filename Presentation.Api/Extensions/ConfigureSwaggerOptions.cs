using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.Api.Extensions;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var desc in _provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = $"Challenge Crud Tasks {desc.GroupName}",
                Version = desc.ApiVersion.ToString(),
                Description = "API for managing Crud Tasks.",
                Contact = new OpenApiContact { Name = "Edwin Ramos Camargo", Email = "edwin.desenv@gmail.com" }
            };

            if (desc.IsDeprecated)
            {
                info.Description += "\n\nNOTE: This API version has been deprecated.";
            }

            options.SwaggerDoc(desc.GroupName, info);
        }
    }
}