using Domain.Entity;

namespace Application.Interface.Service
{
    public interface IBackgroundService
    {
        Task<string> AddTaskSetExpiredSchedule(int taskId, TimeSpan startTime);
        bool RemoveJob(string jobId);
    }
}
