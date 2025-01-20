using Application.DTOs.Task;
using Application.Interface.Service;
using FluentValidation;

namespace Infrastructure.Validators.Task
{
    public class TaskCreateRequestValidator : AbstractValidator<TaskCreateRequest>
    {
        public TaskCreateRequestValidator(ITimeService timeService)
        {
            var utcNow = timeService.GetCurrentUtcDatetime();

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
