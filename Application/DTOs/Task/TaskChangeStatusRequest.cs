using Domain.Enum.Task;

namespace Application.DTOs.Task
{
    public class TaskChangeStatusRequest : BaseTaskUpdateRequest
    {
        public Status Status { get; set; }
    }
}
