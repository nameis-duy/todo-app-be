using Application.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Implement
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction transaction;

        public async Task BeginTransactionAsync()
        {
            transaction = await context.Database.BeginTransactionAsync();
        }

        public async Task<bool> CommitTransactionAsync()
        {
            try
            {
                var affected = await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return affected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public void Dispose() => context.Dispose();
    }
}
