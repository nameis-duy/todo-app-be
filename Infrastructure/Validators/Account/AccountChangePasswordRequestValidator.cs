using Application.DTOs.Account;
using Application.Interface.Service;
using FluentValidation;
using Infrastructure.ExtensionService;

namespace Infrastructure.Validators.Account
{
    public class AccountChangePasswordRequestValidator : AbstractValidator<AccountChangePasswordRequest>
    {
        public AccountChangePasswordRequestValidator(IAccountService accountService, IClaimService claimService)
        {
            var currentUserId = claimService.GetCurrentUserId();

            RuleFor(a => a.OldPassword)
                .CustomAsync(async (password, context, ct) =>
                {
                    var account = await accountService.FindAsync(currentUserId);
                    if (account is null)
                    {
                        context.AddFailure("You are not allowed to do this.");
                        return;
                    }
                    var isValidPassword = password.VerifyPassword(account.PasswordHash);
                    if (isValidPassword is false)
                    {
                        context.AddFailure("Password incorrect");
                        return;
                    }
                });
            RuleFor(a => a.NewPassword)
                .Length(6, 100)
                .Must((dto, password) =>
                {
                    return dto.OldPassword != password;
                })
                .WithMessage("New Password cannot be the same as the old password.");
        }
    }
}
