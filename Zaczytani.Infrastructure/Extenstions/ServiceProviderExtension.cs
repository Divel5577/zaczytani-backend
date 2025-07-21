using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Extenstions;

public static class ServiceProviderExtension
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error while applying migrations: {ex.Message}");
            throw;
        }
    }
}
