using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace WebApi.Configuration;

public static class WebApiConfiguration
{
    public static IServiceCollection ConfigureWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "SimpleMessenger API",
                Description = "A simple messaging API built with ASP.NET Core",
                Contact = new OpenApiContact
                {
                    Name = "SimpleMessenger Team",
                    Email = "info@simplemessenger.com"
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
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
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
        }

        app.MapControllers();

        return app;
    }
} 