using Application.DTOs.Task;
using Application.Interface.Service;
using FluentValidation;

namespace Infrastructure.Validators.Task
{
    public class TaskChangePriorityRequestValidator : AbstractValidator<TaskChangePriorityRequest>
    {
        public TaskChangePriorityRequestValidator(ITimeService timeService,
                                          ITaskService taskService,
                                          IClaimService claimService)
        {
            Include(new BaseTaskUpdateRequestValidator(taskService, claimService));
            RuleFor(t => t.Priority)
                .IsInEnum();
        }
    }
}
