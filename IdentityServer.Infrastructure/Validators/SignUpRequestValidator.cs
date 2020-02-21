using FluentValidation;
using IdentityServer.Infrastructure.RequestModels;

namespace IdentityServer.Infrastructure.Validators
{
    public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpRequestValidator()
        {
            RuleFor(e => e.Username).NotNull().NotEmpty();
            RuleFor(e => e.UserMail).NotNull().NotEmpty().EmailAddress();
            RuleFor(e => e.Password).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(e => e.ConfirmPassword).NotNull().NotEmpty().MinimumLength(3);

            RuleFor(e => e.ConfirmPassword).Equal(e => e.Password);
        }
    }
}