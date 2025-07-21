using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Configuration;
using Zaczytani.Infrastructure.Persistance;
using Zaczytani.Infrastructure.Repositories;
using Zaczytani.Infrastructure.Seeders;


namespace Zaczytani.Infrastructure.Extenstions;

public static class ServiceCollectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ZaczytaniDb");
        services.AddDbContext<BookDbContext>(options => options.UseSqlServer(connectionString).LogTo(Console.WriteLine, LogLevel.Information));

        services.AddIdentityApiEndpoints<User>(options =>
        {
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
            .AddRoles<UserRole>()
            .AddEntityFrameworkStores<BookDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        services.AddSingleton<IFileStorageRepository, FileStorageRepository>();

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookRequestRepository, BookRequestRepository>();
        services.AddScoped<IPublishingHouseRepository, PublishingHouseRepository>();
        services.AddScoped<IUserDrawnBookRepository, UserDrawnBookRepository>();
        services.AddScoped<IBookShelfRepository, BookShelfRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChallengeRepository, ChallengeRepository>();
        services.AddScoped<IEmailInfoRepository, EmailInfoRepository>();
        services.AddScoped<IBadgeRepository, BadgeRepository>();
        services.AddScoped<ISeeder, Seeder>();
        services.AddScoped<SignInManager<User>, CustomLoginHandler>();
    }
}

