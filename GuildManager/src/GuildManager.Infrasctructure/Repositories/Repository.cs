using GuildManager.Domain.Repositories;
using GuildManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GuildManager.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly GuildManagerDbContext _context;
    private readonly DbSet<T> _set;

    public Repository(GuildManagerDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync(new object?[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
