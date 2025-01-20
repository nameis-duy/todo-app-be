using Application.Others;
using System.Linq.Expressions;

namespace Application.Interface.Repository
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll(bool isTracking = false);
        Task<Pagination<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize);
        Task<TEntity?> FindAsync(params object[] keys);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }
}
