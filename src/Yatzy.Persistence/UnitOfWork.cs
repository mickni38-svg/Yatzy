using Yatzy.Application.Interfaces;

namespace Yatzy.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly YatzyDbContext _context;

    public UnitOfWork(YatzyDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
