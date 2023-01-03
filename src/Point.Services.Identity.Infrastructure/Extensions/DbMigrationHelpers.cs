using Point.Services.Identity.Infrastructure.Configuration;

namespace Point.Services.Identity.Infrastructure.Extensions;

public static class DbMigrationHelpers
{

    public static async Task ApplyDbMigrationsWithDataSeedAsync<TIdentityServerDbContext, TUser, TRole, TKey>(
    IHost host, SeedConfiguration seedConfiguration, DatabaseMigrationsConfiguration databaseMigrationsConfiguration)
    where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
    where TUser : IdentityUser<TKey>, new()
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
    {
        using var serviceScope = host.Services.CreateScope();
        var services = serviceScope.ServiceProvider;

        if (databaseMigrationsConfiguration is { ApplyDatabaseMigrations: true })
        {
            host.MigrateDatabase<AspIdentityDbContext>()
                .MigrateDatabase<ServerConfigurationDbContext>()
                .MigrateDatabase<ServerPersistedGrantDbContext>()
                .MigrateDatabase<ProtectionDbContext>();
        }

        if (seedConfiguration is { ApplySeed: true })
        {
            await EnsureSeedDataAsync<TIdentityServerDbContext, TUser, TRole, TKey>(services);
        }
    }

    public static async Task<bool> EnsureSeedDataAsync<TIdentityServerDbContext, TUser, TRole, TKey>(IServiceProvider serviceProvider)
        where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        where TUser : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TIdentityServerDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TRole>>();
        var idsDataConfiguration = scope.ServiceProvider.GetRequiredService<IdentityServerData>();
        var idDataConfiguration = scope.ServiceProvider.GetRequiredService<IdentityData>();

        await EnsureSeedIdentityServerData(context, idsDataConfiguration);
        await EnsureSeedIdentityData<TUser, TRole, TKey>(userManager, roleManager, idDataConfiguration);

        return true;
    }

    /// <summary>
    /// Generate default admin user / role
    /// </summary>
    private static async Task EnsureSeedIdentityData<TUser, TRole, TKey>(UserManager<TUser> userManager,
        RoleManager<TRole> roleManager, IdentityData identityDataConfiguration)
        where TUser : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        // adding roles from seed
        foreach (var r in identityDataConfiguration.Roles)
        {
            if (await roleManager.RoleExistsAsync(r.Name)) continue;

            var role = new TRole
            {
                Name = r.Name
            };

            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded) continue;

            foreach (var claim in r.Claims)
            {
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, claim.Value));
            }
        }

        // adding users from seed
        foreach (var user in identityDataConfiguration.Users)
        {
            var identityUser = new TUser
            {
                UserName = user.Username,
                Email = user.Email,
                EmailConfirmed = true
            };

            var userByUserName = await userManager.FindByNameAsync(user.Username);
            var userByEmail = await userManager.FindByEmailAsync(user.Email);

            // User is already exists in database
            if (userByUserName != default || userByEmail != default)
            {
                continue;
            }

            // if there is no password we create user without password
            // user can reset password later, because accounts have EmailConfirmed set to true
            var result = !string.IsNullOrEmpty(user.Password)
            ? await userManager.CreateAsync(identityUser, user.Password)
            : await userManager.CreateAsync(identityUser);

            if (!result.Succeeded) continue;

            foreach (var claim in user.Claims)
            {
                await userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim(claim.Type, claim.Value));
            }

            foreach (var role in user.Roles)
            {
                await userManager.AddToRoleAsync(identityUser, role);
            }
        }
    }


    /// <summary>
    /// Generate default clients, identity and api resources
    /// </summary>
    private static async Task EnsureSeedIdentityServerData<TIdentityServerDbContext>(TIdentityServerDbContext context, IdentityServerData identityServerDataConfiguration)
    where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
    {
        foreach (var resource in identityServerDataConfiguration.IdentityResources)
        {
            var exits = await context.IdentityResources.AnyAsync(a => a.Name == resource.Name);

            if (exits)
            {
                continue;
            }

            await context.IdentityResources.AddAsync(resource.ToEntity());
        }

        foreach (var apiScope in identityServerDataConfiguration.ApiScopes)
        {
            var exits = await context.ApiScopes.AnyAsync(a => a.Name == apiScope.Name);

            if (exits)
            {
                continue;
            }

            await context.ApiScopes.AddAsync(apiScope.ToEntity());
        }

        foreach (var resource in identityServerDataConfiguration.ApiResources)
        {
            var exits = await context.ApiResources.AnyAsync(a => a.Name == resource.Name);

            if (exits)
            {
                continue;
            }

            foreach (var s in resource.ApiSecrets)
            {
                s.Value = s.Value.ToSha256();
            }

            await context.ApiResources.AddAsync(resource.ToEntity());
        }


        foreach (var client in identityServerDataConfiguration.Clients)
        {
            var exits = await context.Clients.AnyAsync(a => a.ClientId == client.ClientId);

            if (exits)
            {
                continue;
            }

            foreach (var secret in client.ClientSecrets)
            {
                secret.Value = secret.Value.ToSha256();
            }

            client.Claims = client.ClientClaims
                .Select(c => new IdentityServer4.Models.ClientClaim(c.Type, c.Value))
                .ToList();

            await context.Clients.AddAsync(client.ToEntity());
        }

        await context.SaveChangesAsync();
    }
}