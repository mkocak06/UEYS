using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System.Security.Cryptography.X509Certificates;

namespace Shared.Validations;

public class EducationTrackingBaseValidator<T> : AbstractValidator<T> where T : EducationTrackingBase
{
    protected EducationTrackingBaseValidator()
    {
        RuleFor(dto => dto.ProcessType).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProgramId).Must(s => s is not null).When(x => x.ProcessType == ProcessType.Transfer || x.ReasonType == ReasonType.CompletionOfAssignment || x.ReasonType == ReasonType.KKTCHalfTimed || (x.ProcessType == ProcessType.Assignment && x.AssignmentType != AssignmentType.EducationAbroad && x.ReasonType != ReasonType.CompletionOfAssignmentAbroad)).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ReasonType).Must(s => s is not null).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProcessDate).Must(s => s is not null).When(et => (et.ReasonType != ReasonType.ExcusedTransfer || et.ReasonType != ReasonType.UnexcusedTransfer) && et.ProcessType != ProcessType.Start && et.ReasonType != ReasonType.RegistrationByMistake).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.AdditionalDays).Must(s => s > 0).When(et => et.ProcessType == ProcessType.TimeIncreasing || et.ProcessType == ProcessType.TimeDecreasing || et.ReasonType == ReasonType.AnnualLeave || et.ReasonType == ReasonType.RayLeave).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.AdditionalDays).Must(s => s > 0 && s <= 360).When(et => et.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment && et.ProcessType != ProcessType.Start).WithMessage(Resources.Validation.ValidationResource.NotGreaterThan360);
        RuleFor(dto => dto.ExcusedType).Must(s => s is not null).When(et => et.ReasonType == ReasonType.ExcusedTransfer).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.Description).NotEmpty().When(et => et.AssignmentType == AssignmentType.Other || et.ReasonType == ReasonType.TimeDecreasinginAccordanceWithLawNumber1219 || et.ReasonType == ReasonType.TransferFailed || et.ReasonType == ReasonType.TerminationSuspensionofCivilService || et.ReasonType == ReasonType.Other).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.AssignmentType).Must(s => s is not null).When(x => x.ReasonType == ReasonType.LeavingTheInstitutionDueToAssignment).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.EndDate).Must(s => s is not null).When(x => x.ProcessType == ProcessType.TimeIncreasing || x.ReasonType == ReasonType.AnnualLeave || x.ReasonType == ReasonType.RayLeave).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ProcessDate).LessThanOrEqualTo(x => x.EndDate).When(x => x.ProcessType == ProcessType.TimeIncreasing || x.ReasonType == ReasonType.AnnualLeave || x.ReasonType == ReasonType.RayLeave).WithMessage("Bitiş tarihi, başlangıç tarihinden büyük olmalıdır!");
    }
}


public class EducationTrackingValidator : EducationTrackingBaseValidator<EducationTrackingResponseDTO>
{
    public EducationTrackingValidator()
    {
        //RuleFor(dto => dto.Student.ProgramId).Must(s => s.HasValue && s.Value > 0).When(et => et.ReasonId == 31 || et.ReasonId == 32).WithMessage(Resources.Validation.ValidationResource.Required);
    }
}

public class EducationTrackingDTOValidator : EducationTrackingBaseValidator<EducationTrackingDTO>
{
}
