using Application.Interface.Repository;
using Domain.Entity;

namespace Infrastructure.Implement.Repository
{
    public class AccountRepo : GenericRepo<Account>, IAccountRepo
    {
        public AccountRepo(AppDbContext context) : base(context)
        {
        }
    }
}
