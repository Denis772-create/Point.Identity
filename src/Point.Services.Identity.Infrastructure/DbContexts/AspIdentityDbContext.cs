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
        builder.Entity<UserIdentityRole>().ToTable(TableIdentityConsts.Roles);
        builder.Entity<UserIdentityRoleClaim>().ToTable(TableIdentityConsts.RoleClaims);
        builder.Entity<UserIdentityUserRole>().ToTable(TableIdentityConsts.UserRoles);
        builder.Entity<UserIdentity>().ToTable(TableIdentityConsts.Users);
        builder.Entity<UserIdentityLogin>().ToTable(TableIdentityConsts.UserLogins);
        builder.Entity<UserIdentityUserClaim>().ToTable(TableIdentityConsts.UserClaims);
        builder.Entity<UserIdentityToken>().ToTable(TableIdentityConsts.UserTokens);
    }
}