using Application.Others;
using System.Linq.Expressions;

namespace Application.Interface.Repository
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<Pagination<TEntity>> GetPageAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? predicate = null);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }
}
