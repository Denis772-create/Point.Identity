namespace Point.Services.Identity.Infrastructure.Interfaces;

public interface IAdminPersistedGrantDbContext : IPersistedGrantDbContext
{
    DbSet<Key> Keys { get; set; }
}