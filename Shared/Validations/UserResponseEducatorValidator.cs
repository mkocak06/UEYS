using FluentValidation;
using Shared.ResponseModels;
using System;
using System.Linq;

namespace Shared.Validations;

public class UserResponseEducatorValidator : AbstractValidator<UserResponseDTO>
{
    public UserResponseEducatorValidator()
    {
        RuleFor(dto => dto.Email).EmailAddress().WithMessage(Resources.Validation.ValidationResource.NotValidEmail);
        RuleFor(dto => dto.Email).Must(x => !string.IsNullOrEmpty(x)).WithName("E-Mail").WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Phone).Must(x => x.Length == 10 && !string.IsNullOrEmpty(x)).WithName("Telefon").WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Phone).Must(x => x.StartsWith("5")).WithName("Telefon").WithMessage("Lütfen numaranızı başında 5 olacak şekilde (5__) giriniz");

        RuleFor(dto => dto.Educator.AcademicTitleId).Must(x => x.HasValue && x.Value > 0).WithName("Akademik Unvan").WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Educator.StaffTitleId).Must(x => x.HasValue && x.Value > 0).WithName("Kadro Unvan").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        RuleFor(dto => dto.Educator.StaffParentInstitutions).Must(s => s != null && s.Count > 0 && s.Any(x => x.StaffParentInstitutionId != null && x.StaffParentInstitutionId != 0 && x.StartDate != null))
            .WithMessage(Resources.Validation.ValidationResource.StaffParentInstitiution);

        RuleFor(dto => dto.Educator.StaffInstitutions).Must(s => s != null && s.Count > 0 && s.Any(x => x.StaffInstitutionId != null && x.StaffInstitutionId != 0 && x.StartDate != null))
            .WithMessage(Resources.Validation.ValidationResource.StaffInstitution);

        RuleFor(dto => dto.Educator.EducatorAdministrativeTitles).Must(s => s != null && s.Count > 0).WithName("İdari Görev").WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Educator.EducatorPrograms).Must(s => s != null && s.Count > 0).WithName("Uzmanlık Eğitim Programı").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        RuleFor(dto => dto.Educator.IsConditionalEducator).NotNull().WithMessage("Lütfen eğiticinin TUEY Geçici Birinci Madde kapsamında olup olmadığını belirtiniz.");
        RuleFor(dto => dto.Educator.IsForensicMedicineInstitutionEducator).NotNull().WithMessage("Lütfen eğiticinin Adli Tıp Kurumu eğiticisi olup olmadığını belirtiniz.");
        RuleFor(dto => dto.Educator.IsChairman).NotNull().When(x => x.Educator.IsForensicMedicineInstitutionEducator == true).WithMessage("Lütfen eğiticinin başkan mı üye mi olduğunu seçiniz.");
        RuleFor(dto => dto.Educator.ForensicMedicineBoardType).NotNull().When(x => x.Educator.IsChairman == true && x.Educator.IsForensicMedicineInstitutionEducator == true).WithMessage("Lütfen Adli Tıp İhtisas Kurulunu seçiniz.");
        RuleFor(dto => dto.Educator.MembershipStartDate).NotNull().When(x => x.Educator.IsChairman == false && x.Educator.IsForensicMedicineInstitutionEducator == true).WithMessage("Lütfen üyelik başlangıç tarihi seçiniz.");
        RuleFor(dto => dto.Educator.MembershipEndDate).NotNull().When(x => x.Educator.IsChairman == false && x.Educator.IsForensicMedicineInstitutionEducator == true).WithMessage("Lütfen üyelik bitiş tarihi seçiniz.");

        RuleFor(dto => dto.Educator.EducatorExpertiseBranches).Must(expertises =>
        {
            return expertises.All(e => e.ExpertiseBranchId.HasValue && e.ExpertiseBranchId > 0);
        })
        .WithName("Uzmanlık")
        .WithMessage(Resources.Validation.ValidationResource.RequiredField);


        RuleFor(dto => dto.Educator.EducatorExpertiseBranches).Must(expertises =>
        {
            return expertises.All(e => !string.IsNullOrEmpty(e.RegistrationNo) && Convert.ToInt32(e.RegistrationNo) >= 0);
        })
        .When(x => x.Educator.IsConditionalEducator == false)
        .WithName("Tescil Numarası")
        .WithMessage(Resources.Validation.ValidationResource.RequiredField + " ve Tescil numarası alanları negatif değer olamaz.");

        RuleFor(dto => dto.Educator.EducatorExpertiseBranches).Must(expertises =>
        {
            return !expertises.Select(x => x.ExpertiseBranchId).GroupBy(id => id).Any(grp => grp.Count() > 1);
        })
          .WithMessage("Aynı uzmanlık dalından birden fazla eklenemez.");

        RuleFor(dto => dto.Educator.TitleDate).NotNull().When(x => x.Educator.IsConditionalEducator == true).WithName("Doçentlik Belge Tarihi").WithMessage(Resources.Validation.ValidationResource.RequiredField)
            .Must(x => x <= Shared.Extensions.EnumExtension.GetTueyDate()).When(x => x.Educator.IsConditionalEducator == true).WithMessage("Doçentlik Belge Tarihi " + Shared.Extensions.EnumExtension.GetTueyDate().ToShortDateString() + " Tarihinden Küçük Olmalıdır");

        //RuleFor(dto => dto.Educator.Documents).Must(x => x.Any(y => y.DocumentType is DocumentTypes.AssociateProfessorship or DocumentTypes.DeclarationDocument))
        //    .When(x => x.Educator.IsConditionalEducator == true)
        //    .WithMessage(Resources.Validation.ValidationResource.Required);
    }
}