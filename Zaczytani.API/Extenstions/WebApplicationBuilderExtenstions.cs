using Microsoft.OpenApi.Models;
using Zaczytani.Domain.DescriptionEnumConver;

namespace Zaczytani.API.Extenstions;

public static class WebApplicationBuilderExtenstions
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DescriptionEnumConverterFactory());
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                    },
                    []
                }
            });
        });

        var frontendUrl = builder.Configuration.GetSection("FrontendUrl").Value
            ?? throw new InvalidOperationException("Frontend URL is not configured. Please set 'FrontendUrl' in appsettings.json.");

        builder.Services.AddCors(options => options.AddPolicy("frontend",
            policy =>
            {
                policy.WithOrigins(frontendUrl).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
        ));
    }
}
