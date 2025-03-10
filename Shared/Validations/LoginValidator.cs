using FluentValidation;
using Shared.Models;
using Shared.RequestModels;


namespace Shared.Validations
{
    public class LoginValidator : AbstractValidator<UserForLoginDTO>
    {
        public LoginValidator()
        {
            // RuleFor(user => user.Email).EmailAddress().NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.Password).NotEmpty().NotNull().WithName(Resources.Validation.ValidationResource.Password).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);

            // RuleFor(user => user.Email).EmailAddress().When(u => !string.IsNullOrEmpty(u.Email)).WithMessage(Resources.Validation.ValidationResource.NotValidEmail);
        }
    }
}
