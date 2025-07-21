using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zaczytani.Application.Configuration;
using Zaczytani.Application.Http;
using Zaczytani.Application.Mailer;
using Zaczytani.Application.Profiles;
using Zaczytani.Application.Services;
using Zaczytani.Domain.Entities;

namespace Zaczytani.Application.Extenstions;

public static class ServiceCollectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(ServiceCollectionExtension).Assembly;
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(applicationAssembly);

            var dtosProfile = new DtosProfile();
            dtosProfile.Configure(configuration);
            cfg.AddProfile(dtosProfile);
        });
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        services.AddTransient<IEmailSender<User>, EmailSender>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AssignUserIdBehavior<,>));

        services.AddValidatorsFromAssembly(applicationAssembly)
            .AddFluentValidationAutoValidation();

        services.Configure<HttpClientConfig>(configuration.GetSection("BookHttpClient"));
        services.Configure<UserManagementSettings>(configuration.GetSection("UserManagement"));
        services.Configure<MailerSettings>(configuration.GetSection("Smtp"));

        services.AddHttpClient<BookHttpClient>((sp, client) =>
        {
            var config = sp.GetRequiredService<IOptions<HttpClientConfig>>().Value;
            client.BaseAddress = new Uri(config.BaseUrl);
        });

        services.AddScoped<IBadgeAssignerService, BadgeAssignerService>();
    }
}