using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;

namespace Infrastructure.Implement.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        protected readonly IGenericRepo<TEntity> entityRepo;
        protected readonly IUnitOfWork uow;

        public BaseService(IGenericRepo<TEntity> entityRepo, IUnitOfWork uow)
        {
            this.entityRepo = entityRepo;
            this.uow = uow;
        }

        public async Task<TEntity?> FindAsync(params object[] keys)
        {
            return await entityRepo.FindAsync(keys);
        }

        public IQueryable<TEntity> GetAll(bool isTracking = false)
        {
            return entityRepo.GetAll(isTracking);
        }
    }
}
