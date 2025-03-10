
using FluentValidation;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations
{
    public class UpdateUserValidator : AbstractValidator<UserAccountDetailInfoResponseDTO>
    {
        public UpdateUserValidator()
        {
            RuleFor(user => user.Email).EmailAddress().NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithName(Resources.Validation.ValidationResource.Name).WithMessage(Resources.Validation.ValidationResource.RequiredField);
            //RuleFor(user => user.Address).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.BirthDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.BirthPlace).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            //RuleFor(user => user.InstitutionId).NotNull().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.Phone).Must(x => x.Length == 10 && !string.IsNullOrEmpty(x)).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.Phone).Must(x => x.StartsWith("5")).WithMessage("Lütfen numaranızı başında 5 olacak şekilde (5__) giriniz");
            //RuleFor(user => user.RoleIds).Must(x => x?.Count > 0).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        }
    }
}