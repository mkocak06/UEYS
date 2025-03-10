using FluentValidation;
using Shared.RequestModels;

namespace Shared.Validations;

public class AddUserWithStudentInfoValidator : AbstractValidator<AddUserWithStudentInfoDTO>
{
    public AddUserWithStudentInfoValidator()
    {
        RuleFor(dto => dto.Email).EmailAddress().NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithName(Resources.Validation.ValidationResource.Name).WithMessage(Resources.Validation.ValidationResource.RequiredField);
    }
}