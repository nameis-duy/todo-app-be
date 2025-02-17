using System.Linq.Expressions;

namespace Application.Interface.Service
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
    }
}
