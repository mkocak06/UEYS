using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;

namespace Shared.Validations;

public class StudentPerfectionBaseValidator<T> : AbstractValidator<T> where T : StudentPerfectionBase
{
    protected StudentPerfectionBaseValidator()
    {
        RuleFor(dto => dto.IsSuccessful).NotNull().WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.ProcessDate).Must(s => s is not null).WithMessage(ValidationResource.Required);
    }
}

public class StudentPerfectionValidator : StudentPerfectionBaseValidator<StudentPerfectionResponseDTO>
{
    public StudentPerfectionValidator()
    {
        RuleFor(dto => dto.EducatorId).Must(s => s > 0).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.ProgramId).Must(s => s > 0).WithMessage(ValidationResource.Required);
        RuleFor(dto => dto.Experience).Must(s => s > 0).When(x=>x.Perfection?.PerfectionType == Types.PerfectionType.Interventional).WithMessage(ValidationResource.Required);
    }
}

public class StudentPerfectionDTOValidator : StudentPerfectionBaseValidator<StudentPerfectionDTO>
{
}