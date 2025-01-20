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
            var utcNow = timeService.GetCurrentUtcDatetime();

            Include(new BaseTaskUpdateRequestValidator(taskService, claimService));
            RuleFor(t => t.Title)
                .NotNull()
                .Length(6, 150);
            RuleFor(t => t.Description)
                .NotNull()
                .MaximumLength(255);
            RuleFor(t => t.ExpiredAtUtc)
                .GreaterThanOrEqualTo(utcNow.AddMinutes(30));
            RuleFor(t => t.Priority)
                .IsInEnum();
            RuleFor(t => t.Status)
                .IsInEnum();
        }
    }
}
