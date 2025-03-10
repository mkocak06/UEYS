using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;
using System;
using System.Linq;

namespace Shared.Validations;

public class EducatorBaseValidator<T> : AbstractValidator<T> where T : EducatorBase
{
    protected EducatorBaseValidator()
    {

    }
}

public class EducatorValidator : EducatorBaseValidator<EducatorResponseDTO>
{
    public EducatorValidator()
    {
        RuleFor(dto => dto.User.Email).EmailAddress().WithMessage(Resources.Validation.ValidationResource.NotValidEmail);
        RuleFor(dto => dto.User.Email).Must(x => !string.IsNullOrEmpty(x)).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.User.Phone).Must(x => x.Length == 10 && !string.IsNullOrEmpty(x)).WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
        RuleFor(dto => dto.User.Phone).Must(x => x.StartsWith("5")).WithMessage("Lütfen numaranızı başında 5 olacak şekilde (5__) giriniz");

        RuleFor(dto => dto.StaffParentInstitutions).Must(s => s != null && s.Count > 0).WithMessage(Resources.Validation.ValidationResource.Required)
            .Must(s => s.Any(x => x.StaffParentInstitutionId != null && x.StaffParentInstitutionId != 0 && x.StartDate != null)).WithMessage(Resources.Validation.ValidationResource.StaffParentInstitiution);

        RuleFor(dto => dto.StaffInstitutions).Must(s => s != null && s.Count > 0).WithMessage(Resources.Validation.ValidationResource.Required)
           .Must(s => s.Any(x => x.StaffInstitutionId != null && x.StaffInstitutionId != 0 && x.StartDate != null)).WithMessage(Resources.Validation.ValidationResource.StaffInstitution);

        RuleFor(dto => dto.EducatorAdministrativeTitles).Must(s => s != null && s.Count > 0).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(dto => dto.IsConditionalEducator).NotNull().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.IsForensicMedicineInstitutionEducator).NotNull().WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.IsChairman).NotNull().When(x => x.IsForensicMedicineInstitutionEducator == true).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.ForensicMedicineBoardType).NotNull().When(x => x.IsChairman == true).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.MembershipStartDate).NotNull().When(x => x.IsChairman == false).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.MembershipEndDate).NotNull().When(x => x.IsChairman == false).WithMessage(Resources.Validation.ValidationResource.Required);

        RuleFor(dto => dto.EducatorExpertiseBranches).Must(expertises =>
        {
            return expertises.All(e => e.ExpertiseBranchId.HasValue && e.ExpertiseBranchId > 0);
        })
        .WithName("Uzmanlık")
        .WithMessage(Resources.Validation.ValidationResource.RequiredField);


        RuleFor(dto => dto.EducatorExpertiseBranches).Must(expertises =>
            {
                return expertises.All(e => !string.IsNullOrEmpty(e.RegistrationNo) && Convert.ToInt32(e.RegistrationNo) >= 0);
            })
        .When(x => x.IsConditionalEducator == false)
        .WithName("Tescil Numarası")
        .WithMessage(Resources.Validation.ValidationResource.RequiredField + " ve Tescil numarası alanları negatif değer olamaz.");

        RuleFor(dto => dto.EducatorExpertiseBranches).Must(expertises =>
        {
            return !expertises.Select(x => x.ExpertiseBranchId).GroupBy(id => id).Any(grp => grp.Count() > 1);
        })
          .WithMessage("Aynı uzmanlık dalından birden fazla eklenemez.");

        RuleFor(dto => dto.EducatorExpertiseBranches).Must(s => s != null && s.Count > 0).WithMessage(Resources.Validation.ValidationResource.Required);
        RuleFor(dto => dto.EducatorPrograms).Must(s => s != null && s.Count > 0).WithMessage(Resources.Validation.ValidationResource.Required);


        RuleFor(dto => dto.TitleDate).NotNull().When(x => x.IsConditionalEducator == true).WithMessage(Resources.Validation.ValidationResource.Required)
            .Must(x => x <= new DateTime(2009, 7, 18)).When(x => x.IsConditionalEducator == true).WithMessage("Doçentlik Belge Tarihi 18.07.2009 Tarihinden Küçük Olmalıdır");



    }
}

public class EducatorDTOValidator : EducatorBaseValidator<EducatorDTO>
{
}