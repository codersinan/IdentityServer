using FluentValidation;
using IdentityServer.Infrastructure.RequestModels;

namespace IdentityServer.Infrastructure.Validators
{
    public class SignInRequestValidator : AbstractValidator<SignInRequest>
    {
        public SignInRequestValidator()
        {
            RuleFor(e => e.Username).NotNull().NotEmpty();
            RuleFor(e => e.Password).NotNull().NotEmpty().MinimumLength(3);
        }
    }
}