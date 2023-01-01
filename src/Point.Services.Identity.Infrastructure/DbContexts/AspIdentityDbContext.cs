namespace Point.Services.Identity.Infrastructure.DbContexts;

public class AspIdentityDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityLogin, UserIdentityRoleClaim, UserIdentityToken>
{
    public AspIdentityDbContext(DbContextOptions<AspIdentityDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityContext(builder);
    }

    private static void ConfigureIdentityContext(ModelBuilder builder)
    {
        builder.Entity<UserIdentityRole>().ToTable(TableConsts.IdentityRoles);
        builder.Entity<UserIdentityRoleClaim>().ToTable(TableConsts.IdentityRoleClaims);
        builder.Entity<UserIdentityUserRole>().ToTable(TableConsts.IdentityUserRoles);
        builder.Entity<UserIdentity>().ToTable(TableConsts.IdentityUsers);
        builder.Entity<UserIdentityLogin>().ToTable(TableConsts.IdentityUserLogins);
        builder.Entity<UserIdentityUserClaim>().ToTable(TableConsts.IdentityUserClaims);
        builder.Entity<UserIdentityToken>().ToTable(TableConsts.IdentityUserTokens);
    }
}