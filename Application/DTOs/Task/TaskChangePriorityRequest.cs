using Domain.Enum.Task;

namespace Application.DTOs.Task
{
    public class TaskChangePriorityRequest : BaseTaskUpdateRequest
    {
        public Priority Priority { get; set; }
    }
}
