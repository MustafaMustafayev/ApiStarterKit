namespace DAL.EntityFramework.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    public Task<int> CommitAsync(CancellationToken cancellationToken);
    public Task<int> CommitAsync();
    public int Commit();
}