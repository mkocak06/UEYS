using FluentValidation;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class AddUserValidator : AbstractValidator<UserAccountDetailInfoResponseDTO>
{
    public AddUserValidator()
    {
        RuleFor(user => user.UserRoles).Must(x => x != null && x.Count > 0).WithMessage("En az 1 rol girmelisiniz");
        RuleFor(dto => dto.Email).EmailAddress().NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Phone).Must(x => !string.IsNullOrEmpty(x) && x.Length == 10).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Phone).Must(x => x.StartsWith("5")).When(x => !string.IsNullOrEmpty(x.Phone) && x.Phone?.Length == 10).WithMessage("Lütfen numaranızı başında 5 olacak şekilde (5__) giriniz");
        //RuleFor(dto => dto.InstitutionId).NotNull().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        //RuleFor(dto => dto.RoleIds).Must(x => x?.Count > 0).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
    }
}