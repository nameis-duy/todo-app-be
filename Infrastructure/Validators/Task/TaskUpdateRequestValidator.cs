using Application.DTOs.Task;
using Application.Interface.Service;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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
                    var task = await taskService.GetAll().FirstOrDefaultAsync(t => t.Id == dto.Id
                        && !t.IsRemoved, cancellationToken: ct);
                    if (task != null)
                    {
                        if (dto.ExpiredAt.Kind == DateTimeKind.Local)
                        {
                            return task.ExpiredAtUtc != DateTime.SpecifyKind(dto.ExpiredAt, DateTimeKind.Local).ToUniversalTime();
                        }

                        if (dto.ExpiredAt.Kind == DateTimeKind.Utc)
                        {
                            return task.ExpiredAtUtc != dto.ExpiredAt;
                        }
                    }

                    return true;
                });
            RuleFor(t => t.Priority)
                .IsInEnum();
            RuleFor(t => t.Status)
                .IsInEnum();
        }
    }
}
