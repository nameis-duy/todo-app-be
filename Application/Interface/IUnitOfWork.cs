namespace Application.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        Task<bool> CommitTransactionAsync();
        Task<bool> SaveChangeAsync();
    }
}
