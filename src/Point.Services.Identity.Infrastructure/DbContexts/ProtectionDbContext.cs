namespace Point.Services.Identity.Infrastructure.DbContexts;

public class ProtectionDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public ProtectionDbContext(DbContextOptions<ProtectionDbContext> options)
        : base(options) { }
}
