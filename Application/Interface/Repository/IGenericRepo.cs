using Application.Others;
using System.Linq.Expressions;

namespace Application.Interface.Repository
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll(bool isTracking = false);
        Task<Pagination<TEntity>> GetPageAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? predicate = null);
        Task<TEntity?> FindAsync(params object[] keys);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }
}
