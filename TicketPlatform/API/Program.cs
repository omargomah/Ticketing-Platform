
using API.ExceptionsHandlers;
using API.SwaggerConfigurations;
using Application;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Infrastructure;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Serilog
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
            #endregion
           
            try
            {
                Log.Information("Starting web application");

                var builder = WebApplication.CreateBuilder(args);
            
                // Add services of Other Projects
                builder.Services.AddInfrastructureRegisteration(builder.Configuration);
                builder.Services.AddApplicationRegistrations();

                builder.Services.AddProblemDetails();
                builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            
                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
            

                #region Swagger
                builder.Services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("x-version"));
                }).AddApiExplorer( options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = false;
                });


                builder.Services.AddSwaggerGen(options =>
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                });
                builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

                #endregion


                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();

                    #region Swagger
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                        foreach (var desc in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                                                    desc.GroupName.ToUpperInvariant());
                        }
                    });
                    #endregion

                }

                app.UseHttpsRedirection();
            
                app.UseAuthorization();
                app.UseExceptionHandler();

                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
