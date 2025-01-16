using Application.Interface.Repository;
using Domain.Entity;

namespace Infrastructure.Implement.Repository
{
    public class TaskRepo : GenericRepo<Tasks>, ITaskRepo
    {
        public TaskRepo(AppDbContext context) : base(context)
        {
        }
    }
}
