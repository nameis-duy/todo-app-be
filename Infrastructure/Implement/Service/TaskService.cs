using Application.DTOs.Base;
using Application.DTOs.Task;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Application.Others;
using Domain.Entity;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implement.Service
{
    public class TaskService : BaseService<Tasks>, ITaskService
    {
        private readonly IClaimService claimService;
        private readonly int currentUserId;

        public TaskService(IGenericRepo<Tasks> entityRepo,
                           IUnitOfWork uow,
                           IClaimService claimService) : base(entityRepo, uow)
        {
            this.claimService = claimService;
            currentUserId = claimService.GetCurrentUserId();
        }

        public async Task<ResponseResult<TaskVM>> CreateTaskAsync(TaskCreateRequest dto)
        {
            var task = dto.Adapt<Tasks>();
            task.CreatedBy = currentUserId;
            if (currentUserId == -1) throw new UnauthorizedAccessException("You are not allowed to do this.");

            await uow.BeginTransactionAsync();
            await entityRepo.AddAsync(task);
            var isSucceed = await uow.CommitTransactionAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new DbUpdateException("Create tasks failed. Server error.");
        }

        public async Task<ResponseResult<IEnumerable<TaskVM>>> GetAllTasks()
        {
            var tasks = await entityRepo.GetAll()
                .Where(t => t.CreatedBy == currentUserId
                && !t.IsRemoved)
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.Priority)
                .ThenBy(t => t.CreatedAtUtc)
                .ToListAsync();

            return new ResponseResult<IEnumerable<TaskVM>>
            {
                Data = tasks.Adapt<IEnumerable<TaskVM>>(),
                IsSucceed = true
            };
        }

        public async Task<Pagination<TaskVM>> GetPageAsync(int pageIndex = 0, int pageSize = 10)
        {
            var tasksPage = await entityRepo.GetPageAsync(pageIndex, pageSize,
                t => t.CreatedBy == currentUserId
                && !t.IsRemoved);

            return tasksPage.Adapt<Pagination<TaskVM>>();
        }

        public async Task<ResponseResult<TaskVM?>> GetTaskById(int id)
        {
            var task = await entityRepo.GetAll()
                .FirstOrDefaultAsync(t => t.Id == id
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
            var task = await entityRepo.FindAsync(dto.Id);
            task!.IsRemoved = true;

            await uow.BeginTransactionAsync();
            entityRepo.Update(task!);
            var isSucceed = await uow.CommitTransactionAsync();
            if (isSucceed) return new ResponseResult<int>
            {
                Data = task.Id,
                IsSucceed = isSucceed,
            };
            throw new DbUpdateException("Update tasks failed. Server error.");
        }

        public async Task<ResponseResult<TaskVM>> UpdateTaskAsync(TaskUpdateRequest dto)
        {
            var task = await entityRepo.FindAsync(dto.Id);
            dto.Adapt(task);

            await uow.BeginTransactionAsync();
            entityRepo.Update(task!);
            var isSucceed = await uow.CommitTransactionAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new DbUpdateException("Update tasks failed. Server error.");
        }

        public async Task<ResponseResult<TaskVM>> UpdateTaskPriorityAsync(TaskChangePriorityRequest dto)
        {
            var task = await entityRepo.FindAsync(dto.Id);
            task!.Priority = dto.Priority;

            await uow.BeginTransactionAsync();
            entityRepo.Update(task!);
            var isSucceed = await uow.CommitTransactionAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new DbUpdateException("Update tasks failed. Server error.");
        }

        public async Task<ResponseResult<TaskVM>> UpdateTaskStatusAsync(TaskChangeStatusRequest dto)
        {
            var task = await entityRepo.FindAsync(dto.Id);
            task!.Status = dto.Status;

            await uow.BeginTransactionAsync();
            entityRepo.Update(task!);
            var isSucceed = await uow.CommitTransactionAsync();
            if (isSucceed) return new ResponseResult<TaskVM>
            {
                Data = task.Adapt<TaskVM>(),
                IsSucceed = isSucceed,
            };
            throw new DbUpdateException("Update tasks failed. Server error.");
        }
    }
}
