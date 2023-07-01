namespace Point.Services.Identity.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    public static void RegisterSqlServerDbContexts<TIdentityDbContext, TConfigurationDbContext,
        TPersistedGrantDbContext, TDataProtectionDbContext>(this IServiceCollection services,
        string identityConnectionString, string configurationConnectionString,
        string persistedGrantConnectionString, string dataProtectionConnectionString)
        where TIdentityDbContext : DbContext
        where TPersistedGrantDbContext : DbContext, IPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

        // Config DB for identity
        services.AddDbContext<TIdentityDbContext>(options => 
            options.UseSqlServer(identityConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

        // Config DB from existing connection
        services.AddConfigurationDbContext<TConfigurationDbContext>(options =>
            options.ConfigureDbContext = b =>
                b.UseSqlServer(configurationConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

        // Operational DB from existing connection
        services.AddOperationalDbContext<TPersistedGrantDbContext>(options =>
            options.ConfigureDbContext = b =>
                b.UseSqlServer(persistedGrantConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

        // DataProtectionKey DB from existing connection
        services.AddDbContext<TDataProtectionDbContext>(options =>
            options.UseSqlServer(dataProtectionConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
    }
}