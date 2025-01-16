using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Domain.Entity;

namespace Infrastructure.Implement.Service
{
    public class TaskService : BaseService<Tasks>, ITaskService
    {
        public TaskService(IGenericRepo<Tasks> entityRepo, IUnitOfWork uow) : base(entityRepo, uow)
        {
        }
    }
}
