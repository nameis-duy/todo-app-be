using Application.Others;
using System.Linq.Expressions;

namespace Application.Interface.Service
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        Task<TEntity?> FindAsync(params object[] keys);
        IQueryable<TEntity> GetAll(bool isTracking = false);
    }
}
