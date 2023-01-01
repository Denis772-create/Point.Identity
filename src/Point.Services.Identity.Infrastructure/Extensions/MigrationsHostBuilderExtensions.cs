using Microsoft.Extensions.Hosting;

namespace Point.Services.Identity.Infrastructure.Extensions;

public static class MigrationsHostBuilderExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host)
        where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
        dbContext.Database.Migrate();

        return host;
    }
}