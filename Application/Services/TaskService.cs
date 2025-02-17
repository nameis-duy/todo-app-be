using Application.DTOs.Base;
using Application.DTOs.Task;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Application.Others;
using Domain.Entity;
using Mapster;

namespace Application.Services
{
    public class TaskService : BaseService<Tasks>, ITaskService
    {
        private readonly ITaskRepo taskRepo;
        private readonly IClaimService claimService;
        private readonly int currentUserId;

        public TaskService(ITaskRepo taskRepo,
                           IUnitOfWork uow,
                           IClaimService claimService) : base(taskRepo, uow)
        {
            this.taskRepo = taskRepo;
            this.claimService = claimService;
            currentUserId = claimService.GetCurrentUserId();
        }

        public async Task<ResponseResult<TaskVM>> CreateTaskAsync(TaskCreateRequest dto)
        {
            var task = dto.Adapt<Tasks>();
            task.CreatedBy = currentUserId;
            if (currentUserId == -1) throw new UnauthorizedAccessException("You are not allowed to do this.");

            await taskRepo.AddAsync(task);
            var isSucceed = await uow.SaveChangeAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new SystemException("Create tasks failed. Server error.");
        }

        public async Task<ResponseResult<IEnumerable<TaskVM>>> GetAllTasksAsync()
        {
            var tasks = await taskRepo.GetAllAsync();
            //.Where(t => t.CreatedBy == currentUserId
            //&& !t.IsRemoved)
            //.OrderBy(t => t.Status)
            //.ThenByDescending(t => t.Priority)
            //.ThenBy(t => t.CreatedAt)
            //.ToListAsync();

            return new ResponseResult<IEnumerable<TaskVM>>
            {
                Data = tasks.Adapt<IEnumerable<TaskVM>>(),
                IsSucceed = true
            };
        }

        public async Task<Pagination<TaskVM>> GetPageAsync(int pageIndex = 0, int pageSize = 10)
        {
            var tasksPage = await taskRepo.GetPageAsync(pageIndex, pageSize,
                t => t.CreatedBy == currentUserId
                && !t.IsRemoved);

            return tasksPage.Adapt<Pagination<TaskVM>>();
        }

        public async Task<ResponseResult<TaskVM?>> GetTaskById(int id)
        {
            var task = await taskRepo.FirstOrDefaultAsync(t => t.Id == id
                && !t.IsRemoved
                && t.CreatedBy == currentUserId);

            return new ResponseResult<TaskVM?>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = true,
            };
        }

        public async Task<ResponseResult<int>> RemoveTaskAsync(TaskRemoveRequest dto)
        {
            var task = await taskRepo.FirstOrDefaultAsync(t => dto.Id == t.Id);
            task!.IsRemoved = true;

            taskRepo.Update(task!);
            var isSucceed = await uow.SaveChangeAsync();
            if (isSucceed) return new ResponseResult<int>
            {
                Data = task.Id,
                IsSucceed = isSucceed,
            };
            throw new SystemException("Update tasks failed. Server error.");
        }

        public async Task<ResponseResult<TaskVM>> UpdateTaskAsync(TaskUpdateRequest dto)
        {
            var task = await taskRepo.FirstOrDefaultAsync(t => dto.Id == t.Id);
            dto.Adapt(task);

            taskRepo.Update(task!);
            var isSucceed = await uow.SaveChangeAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new SystemException("Update tasks failed. Server error.");
        }

        public async Task<ResponseResult<TaskVM>> UpdateTaskPriorityAsync(TaskChangePriorityRequest dto)
        {
            var task = await taskRepo.FirstOrDefaultAsync(t => dto.Id == t.Id);
            task!.Priority = dto.Priority;

            taskRepo.Update(task!);
            var isSucceed = await uow.SaveChangeAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new SystemException("Update tasks failed. Server error.");
        }

        public async Task<ResponseResult<TaskVM>> UpdateTaskStatusAsync(TaskChangeStatusRequest dto)
        {
            var task = await taskRepo.FirstOrDefaultAsync(t => dto.Id == t.Id);
            task!.Status = dto.Status;

            taskRepo.Update(task!);
            var isSucceed = await uow.SaveChangeAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new SystemException("Update tasks failed. Server error.");
        }
    }
}
