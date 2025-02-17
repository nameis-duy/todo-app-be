using Application.DTOs.Authenticate;
using Application.Interface.Service;
using FluentValidation;

namespace Application.Validators.Authenticate
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator(IAccountService accountService)
        {
            RuleFor(r => r.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .CustomAsync(async (email, context, ct) =>
                {
                    var isExistEmail = await accountService.FindAsync(e => e.Email.ToLower() == email.ToLower()) != null;
                    if (isExistEmail)
                    {
                        context.AddFailure("Email already exists");
                        return;
                    }
                });
            RuleFor(r => r.Password)
                .Length(6, 100);
            RuleFor(r => r.FirstName)
                .NotEmpty()
                .MaximumLength(250);
            RuleFor(r => r.LastName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
