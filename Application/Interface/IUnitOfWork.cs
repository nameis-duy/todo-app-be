namespace Application.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransaction();
        Task<bool> CommitTransaction();
    }
}
