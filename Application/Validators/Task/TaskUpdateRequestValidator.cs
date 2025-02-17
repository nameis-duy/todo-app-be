using Application.DTOs.Task;
using Application.Interface.Service;
using FluentValidation;

namespace Infrastructure.Validators.Task
{
    public class TaskUpdateRequestValidator : AbstractValidator<TaskUpdateRequest>
    {
        public TaskUpdateRequestValidator(ITimeService timeService,
                                          ITaskService taskService,
                                          IClaimService claimService)
        {
            var now = timeService.GetCurrentLocalDateTime();

            Include(new BaseTaskUpdateRequestValidator(taskService, claimService));
            RuleFor(t => t.Title)
                .NotNull()
                .Length(6, 255);
            RuleFor(t => t.Description)
                .NotNull();
            RuleFor(t => t.ExpiredAt)
                .GreaterThanOrEqualTo(now.AddMinutes(30))
                .WhenAsync(async (dto, context, ct) =>
                {
                    var task = await taskService.FindAsync(t => t.Id == dto.Id
                        && !t.IsRemoved);
                    if (task != null) return task.ExpiredAt != dto.ExpiredAt;
                    return true;
                });
            RuleFor(t => t.Priority)
                .IsInEnum();
            RuleFor(t => t.Status)
                .IsInEnum();
        }
    }
}
