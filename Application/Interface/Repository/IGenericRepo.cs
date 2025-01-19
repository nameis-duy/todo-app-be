namespace Application.Interface.Repository
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll(bool isTracking = false);
        Task<TEntity?> FindAsync(params object[] keys);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }
}
