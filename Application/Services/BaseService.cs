using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using System.Linq.Expressions;

namespace Application.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        protected readonly IUnitOfWork uow;
        private readonly IGenericRepo<TEntity> entityRepo;

        public BaseService(IGenericRepo<TEntity> entityRepo, IUnitOfWork uow)
        {
            this.entityRepo = entityRepo;
            this.uow = uow;
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await entityRepo.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await entityRepo.GetAllAsync(predicate);
        }
    }
}
