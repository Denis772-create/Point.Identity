namespace Point.Services.Identity.Infrastructure.DbContexts;

public class ServerPersistedGrantDbContext : PersistedGrantDbContext<ServerPersistedGrantDbContext>, IAdminPersistedGrantDbContext
{
    public ServerPersistedGrantDbContext(DbContextOptions<ServerPersistedGrantDbContext> options)
        : base(options, new OperationalStoreOptions())
    {
    }

    public DbSet<Key> Keys { get; set; }
}
