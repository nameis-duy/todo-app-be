using Application.DTOs.Base;
using Application.DTOs.Task;
using Domain.Entity;

namespace Application.Interface.Service
{
    public interface ITaskService : IBaseService<Tasks>
    {
        Task<ResponseResult<int>> CreateTaskAsync(TaskCreateRequest dto);
        Task<ResponseResult<TaskVM>> UpdateTaskAsync(TaskUpdateRequest dto);
        Task<ResponseResult<TaskVM>> UpdateTaskStatusAsync(TaskChangeStatusRequest dto);
        Task<ResponseResult<TaskVM>> UpdateTaskPriorityAsync(TaskChangePriorityRequest dto);
        Task<ResponseResult<int>> RemoveTaskAsync(TaskRemoveRequest dto);
    }
}
