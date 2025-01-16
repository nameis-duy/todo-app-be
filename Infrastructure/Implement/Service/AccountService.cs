using Application.Interface.Repository;
using Application.Interface.Service;
using Domain.Entity;

namespace Infrastructure.Implement.Service
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        public AccountService(IGenericRepo<Account> entityRepo) : base(entityRepo)
        {
        }
    }
}
