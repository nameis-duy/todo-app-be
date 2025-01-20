using Application.Interface.Repository;
using Application.Others;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public async Task<Pagination<TEntity>> GetPageAsync(Expression<Func<TEntity, bool>>? predicate,
                                                            int pageIndex,
                                                            int pageSize)
        {
            var source = context.Set<TEntity>();
            if (predicate is not null) source.Where(predicate);
            var totalCount = await source.CountAsync();
            var items = await source.AsNoTracking()
                .Skip(pageIndex * pageSize).Take(pageSize)
                .ToListAsync();
            var result = new Pagination<TEntity>()
            {
                Items = items,
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalItems = totalCount
            };
            return result;
        }

        public void Update(TEntity entity)
        {
            context.Set<TEntity>().Update(entity);
        }
    }
}
