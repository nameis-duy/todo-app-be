using Application.DTOs.Task;
using Application.Interface.Service;
using FluentValidation;

namespace Infrastructure.Validators.Task
{
    public class BaseTaskUpdateRequestValidator : AbstractValidator<BaseTaskUpdateRequest>
    {
        public BaseTaskUpdateRequestValidator(ITaskService taskService,
                                              IClaimService claimService)
        {
            var currentUserId = claimService.GetCurrentUserId();

            RuleFor(t => t.Id)
                .CustomAsync(async (id, context, ct) =>
                {
                    var isExistTask = await taskService.FindAsync(t => t.Id == id
                        && !t.IsRemoved
                        && t.CreatedBy == currentUserId) != null;
                    if (isExistTask is false)
                    {
                        context.AddFailure("Task does not exists");
                        return;
                    }
                });
        }
    }
}
