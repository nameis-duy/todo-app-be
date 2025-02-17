using Application.DTOs.Account;
using Application.Interface.Service;
using FluentValidation;

namespace Application.Validators.Account
{
    public class AccountChangePasswordRequestValidator : AbstractValidator<AccountChangePasswordRequest>
    {
        public AccountChangePasswordRequestValidator(IAccountService accountService, IClaimService claimService,
                                                     IPasswordHelper passwordHelper)
        {
            var currentUserId = claimService.GetCurrentUserId();

            RuleFor(a => a.OldPassword)
                .CustomAsync(async (password, context, ct) =>
                {
                    var account = await accountService.FindAsync(acc => acc.Id == currentUserId);
                    if (account is null)
                    {
                        context.AddFailure("You are not allowed to do this.");
                        return;
                    }
                    var isValidPassword = passwordHelper.VerifyPassword(password, account.PasswordHash);
                    if (isValidPassword is false)
                    {
                        context.AddFailure("Incorrect password");
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
