using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class StudentBaseValidator<T> : AbstractValidator<T> where T : StudentBase
{
    protected StudentBaseValidator()
    {

    }
}

public class StudentValidator : StudentBaseValidator<StudentResponseDTO>
{
    public StudentValidator()
    {
        RuleFor(dto => dto.User.Name).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.User.Email).EmailAddress().NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.Curriculum).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class StudentDTOValidator : StudentBaseValidator<StudentDTO>
{
}