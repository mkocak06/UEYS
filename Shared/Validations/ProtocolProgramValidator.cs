using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;
using System.Linq;

namespace Shared.Validations;

public class ProtocolProgramValidator : AbstractValidator<ProtocolProgramResponseDTO>
{
    public ProtocolProgramValidator()
    {
        RuleFor(dto => dto.ProtocolNo).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.ParentProgram).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.DecisionDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.DecisionNo).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleForEach(dto => dto.RelatedDependentPrograms).SetValidator(new RelatedDependentProgramValidator()).When(x=>x.Type == ProgramType.Protocol);
        //RuleForEach(dto => dto.RelatedDependentPrograms).Must(x => x.DecisionDate != null).When(x => x.Type == ProgramType.Protocol).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        //RuleForEach(dto => dto.RelatedDependentPrograms).Must(x => x.DecisionNo != null).When(x => x.Type == ProgramType.Protocol).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
    }
}

public class RelatedDependentProgramValidator : AbstractValidator<RelatedDependentProgramResponseDTO>
{
    public RelatedDependentProgramValidator()
    {
        RuleFor(dto => dto.DecisionDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.DecisionNo).NotNull().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
    }
}
