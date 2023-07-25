using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using APPLICATION.INFRAESTRUTURE.REPOSITORY.EVENTS;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY;

public class UnitOfWork : IUnitOfWork
{
    private readonly Context _context;

    public IEventRepository EventRepository { get; }

    public UnitOfWork(
        Context context)
    {
        _context = context;

        EventRepository = new EventRepository(_context);
    }

    public async Task CommitAsync()
        => await _context.SaveChangesAsync();

    public async Task RollbackAsync() 
        => await _context.DisposeAsync();
}
