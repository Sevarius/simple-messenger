using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebApi.Hubs;

namespace WebApi.Configuration;

public static class WebApiConfiguration
{
    public static IServiceCollection ConfigureWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddSignalR();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "SimpleMessenger API"
            });
            
            swaggerGenOptions.AddSignalRSwaggerGen(signalRSwaggerGenOptions => signalRSwaggerGenOptions.ScanAssemblies(Assembly.GetExecutingAssembly()));
        });

        return services;
    }

    public static WebApplication UseWebApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleMessenger API V1");
                c.RoutePrefix = "swagger";
            });
        }

        app.MapHub<CommandHub>("api/commandHub");

        app.MapControllers();

        return app;
    }
} 