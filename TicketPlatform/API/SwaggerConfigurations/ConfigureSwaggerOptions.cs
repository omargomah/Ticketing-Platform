using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerConfigurations
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var desc in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(desc.GroupName, new OpenApiInfo
                {
                    Title = "Weather API",
                    Version = desc.ApiVersion.ToString(),
                    Description = desc.IsDeprecated ? "Deprecated" : "Stable",
                    Contact = new OpenApiContact()
                    {
                        Email = "mrjmh934@gmail.com",
                        Name = "Omar Gomaa",
                        Url = new Uri("https://github.com/omargomah")
                    },
                    Summary = "Add the Summary or the Project"
                });
            }
        }
    }
}