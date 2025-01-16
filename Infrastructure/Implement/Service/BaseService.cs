using Application.Interface.Repository;
using Application.Interface.Service;

namespace Infrastructure.Implement.Service
{
    public class BaseService<TEntity>(IGenericRepo<TEntity> entityRepo) : IBaseService<TEntity> where TEntity : class
    {
        public async Task<TEntity?> FindAsync(params object[] keys)
        {
            return await entityRepo.FindAsync(keys);
        }

        public IQueryable<TEntity> GetAll()
        {
            return entityRepo.GetAll();
        }
    }
}
