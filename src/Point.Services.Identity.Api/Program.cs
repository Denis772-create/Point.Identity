namespace Point.Services.Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        Log.Logger = CreateSerilogLogger(configuration);

        try
        {
            Log.Information("Configuring web host ({ApplicationName})...");
            var host = BuildHost(configuration, args);

            Log.Information("Starting web host ({ApplicationName})...");
            host.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationName})!");
        }
    }

    public static IHost BuildHost(IConfiguration configuration, string[] args) =>
        Host.CreateDefaultBuilder(args)
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
        var aspEnviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{aspEnviroment}.json",
                optional: false, reloadOnChange: true)
            .Build();
    }

    private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        => new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
}