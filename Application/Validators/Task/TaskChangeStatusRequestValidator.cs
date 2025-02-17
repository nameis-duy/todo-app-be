using Application.DTOs.Task;
using Application.Interface.Service;
using FluentValidation;

namespace Infrastructure.Validators.Task
{
    public class TaskChangeStatusRequestValidator : AbstractValidator<TaskChangeStatusRequest>
    {
        public TaskChangeStatusRequestValidator(ITimeService timeService,
                                          ITaskService taskService,
                                          IClaimService claimService)
        {
            Include(new BaseTaskUpdateRequestValidator(taskService, claimService));
            RuleFor(t => t.Status)
                .IsInEnum();
        }
    }
}
