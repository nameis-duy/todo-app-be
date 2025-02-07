using Application.Constant;
using Application.DTOs.Account;
using Application.Interface.Service;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validators.Account
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
