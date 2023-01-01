using Point.Services.Identity.Domain;

namespace Point.Services.Identity.Infrastructure.DbContexts;

public class IdentityServerPersistedGrantDbContext : PersistedGrantDbContext<IdentityServerPersistedGrantDbContext>, IAdminPersistedGrantDbContext
{
    public IdentityServerPersistedGrantDbContext(DbContextOptions<IdentityServerPersistedGrantDbContext> options)
        : base(options, new OperationalStoreOptions())
    {
    }

    public DbSet<Key> Keys { get; set; }
}
