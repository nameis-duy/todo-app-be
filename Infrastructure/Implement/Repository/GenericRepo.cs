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

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            if (predicate is null) return await context.Set<TEntity>().ToListAsync();
            return await context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<Pagination<TEntity>> GetPageAsync(int pageIndex,
                                                            int pageSize,
                                                            Expression<Func<TEntity, bool>>? predicate = null)
        {
            var source = context.Set<TEntity>().AsQueryable();
            if (predicate is not null) source = source.Where(predicate);
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
