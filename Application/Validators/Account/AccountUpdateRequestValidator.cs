using Application.Constant;
using Application.DTOs.Account;
using FluentValidation;

namespace Application.Validators.Account
{
    public class AccountUpdateRequestValidator : AbstractValidator<AccountUpdateRequest>
    {
        public AccountUpdateRequestValidator()
        {
            var phoneFormat = ConfigConstant.PHONE_FORMAT;

            RuleFor(a => a.FirstName)
                .NotEmpty()
                .MaximumLength(250);
            RuleFor(a => a.LastName)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(a => a.Phone)
                .Matches(phoneFormat)
                .When(a => !string.IsNullOrEmpty(a.Phone));
        }
    }
}
