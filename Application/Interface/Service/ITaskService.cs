using Application.DTOs.Base;
using Application.DTOs.Task;
using Application.Others;
using Domain.Entity;

namespace Application.Interface.Service
{
    public interface ITaskService : IBaseService<Tasks>
    {
        Task<ResponseResult<TaskVM?>> GetTaskById(int id);
        Task<ResponseResult<IEnumerable<TaskVM>>> GetAllTasks();
        Task<Pagination<TaskVM>> GetPageAsync(int pageIndex = 0, int pageSize = 10);
        Task<ResponseResult<TaskVM>> CreateTaskAsync(TaskCreateRequest dto);
        Task<ResponseResult<TaskVM>> UpdateTaskAsync(TaskUpdateRequest dto);
        Task<ResponseResult<TaskVM>> UpdateTaskStatusAsync(TaskChangeStatusRequest dto);
        Task<ResponseResult<TaskVM>> UpdateTaskPriorityAsync(TaskChangePriorityRequest dto);
        Task<ResponseResult<int>> RemoveTaskAsync(TaskRemoveRequest dto);
    }
}
