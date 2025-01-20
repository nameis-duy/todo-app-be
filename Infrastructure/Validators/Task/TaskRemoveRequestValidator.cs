using Application.DTOs.Task;
using Application.Interface.Service;
using FluentValidation;

namespace Infrastructure.Validators.Task
{
    public class TaskRemoveRequestValidator : AbstractValidator<TaskRemoveRequest>
    {
        public TaskRemoveRequestValidator(ITimeService timeService,
                                          ITaskService taskService,
                                          IClaimService claimService)
        {
            Include(new BaseTaskUpdateRequestValidator(taskService, claimService));
        }
    }
}
