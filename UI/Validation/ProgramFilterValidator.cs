using FluentValidation;
using UI.Models.FilterModels;

namespace UI.Validation
{
    public class ProgramFilterValidator : AbstractValidator<ProgramFilter>
    {
        public ProgramFilterValidator()
        {
            RuleFor(x => x).Must(x=> !((x.UniversityList is null || x.UniversityList.Count <= 0) && (x.HospitalList is null || x.HospitalList.Count <= 0) && (x.ProfessionList is null || x.ProfessionList.Count <= 0) && (x.ExpertiseBranchList is null || x.ExpertiseBranchList.Count <= 0))).WithMessage("En az bir alan dolu olmalıdır");
        }
    }
}

