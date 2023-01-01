using Point.Services.Identity.Infrastructure.Configuration;

namespace Point.Services.Identity.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = GetConfiguration();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Configuring web host ({ApplicationName})...");
            var host = BuildHost(args);

            await ApplyDbMigrationsWithDataSeedAsync(configuration, host);

            Log.Information("Starting web host ({ApplicationName})...");
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationName})!");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }


    public static IHost BuildHost(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.AddEnvironmentVariables();
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseSerilog()
            .Build();

    private static IConfiguration GetConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private static async Task ApplyDbMigrationsWithDataSeedAsync(IConfiguration configuration,
        IHost host)
    {
        var seedConfiguration = configuration.GetSection(nameof(SeedConfiguration)).Get<SeedConfiguration>();
        var databaseMigrationsConfiguration = configuration.GetSection(nameof(DatabaseMigrationsConfiguration))
            .Get<DatabaseMigrationsConfiguration>();

        await DbMigrationHelpers
            .ApplyDbMigrationsWithDataSeedAsync<IdentityServerConfigurationDbContext, AspIdentityDbContext,
                IdentityServerPersistedGrantDbContext, ProtectionDbContext, UserIdentity, UserIdentityRole, Guid>(host,
                seedConfiguration, databaseMigrationsConfiguration);
    }
}