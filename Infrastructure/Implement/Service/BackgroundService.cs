using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Domain.Entity;
using Domain.Enum.Task;
using Hangfire;

namespace Infrastructure.Implement.Service
{
    public class BackgroundService : IBackgroundService
    {
        private readonly ITaskRepo _taskRepo;
        private readonly IUnitOfWork uow;

        public BackgroundService(ITaskRepo taskRepo, IUnitOfWork uow)
        {
            _taskRepo = taskRepo;
            this.uow = uow;
        }

        #region AddTaskSetExpiredSchedule
        public async Task<string> AddTaskSetExpiredSchedule(int taskId, TimeSpan startTime)
        {
            var task = await _taskRepo.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) return string.Empty;
            if (task.Status == Status.InProcess || task.Status == Status.Completed) return string.Empty;

            return BackgroundJob.Schedule(() => ChangeTaskStatusAsync(task), startTime);
        }

        public async Task ChangeTaskStatusAsync(Tasks task)
        {
            task.Status = Status.InProcess;
            _taskRepo.Update(task);
            await uow.SaveChangeAsync();
        }
        #endregion

        public bool RemoveJob(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }
    }
}
