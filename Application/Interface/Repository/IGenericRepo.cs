namespace Application.Interface.Repository
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity?> FindAsync(params object[] keys);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }
}
