using Application.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implement.Repository
{
    public class GenericRepo<TEntity>(AppDbContext context) : IGenericRepo<TEntity> where TEntity : class
    {
        public async Task AddAsync(TEntity entity)
        {
            await context.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
        }

        public async Task<TEntity?> FindAsync(params object[] keys)
        {
            return await context.Set<TEntity>().FindAsync(keys);
        }

        public IQueryable<TEntity> GetAll(bool isTracking = false)
        {
            if (isTracking) return context.Set<TEntity>().AsQueryable();
            return context.Set<TEntity>().AsNoTracking();
        }

        public void Update(TEntity entity)
        {
            context.Set<TEntity>().Update(entity);
        }
    }
}
