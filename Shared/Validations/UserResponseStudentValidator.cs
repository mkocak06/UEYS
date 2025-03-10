using FluentValidation;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;

namespace Shared.Validations;

public class UserResponseStudentValidator : AbstractValidator<UserResponseDTO>
{
    public UserResponseStudentValidator()
    {
        RuleFor(dto => dto.Email).EmailAddress().WithMessage(Resources.Validation.ValidationResource.NotValidEmail);
        RuleFor(dto => dto.Email).Must(x => !string.IsNullOrEmpty(x)).WithName("Email").WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Phone).Must(x => x.Length == 10 && !string.IsNullOrEmpty(x)).WithName("Telefon").WithMessage(Resources.Validation.ValidationResource.RequiredField);
        RuleFor(dto => dto.Phone).Must(x => x.StartsWith("5")).WithMessage("Lütfen numaranızı başında 5 olacak şekilde (5__) giriniz");

        RuleFor(dto => dto.Student.StartReason).NotNull().WithMessage("Başlangıç nedeni zorunludur!");
        RuleFor(dto => dto.Student.ProgramId).Must(x => x.HasValue && x.Value > 0).WithName("Mevcut Kurumu ve Uzmanlık Eğitim Programı").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        RuleFor(dto => dto.Student.BeginningExam).Must(x => x.HasValue).When(x => x.Student.ProgramId != null && x.Student.ProgramId > 0)
            .WithName("Yerleştirildiği Sınav").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        RuleFor(dto => dto.Student.PlacementScore).Must(x => x.HasValue && x >= 0 && x <= 100).When(x => x.Student.ProgramId != null && x.Student.ProgramId > 0)
            .WithName("Yerleştirme Puanı").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        RuleFor(dto => dto.Student.BeginningPeriod).Must(x => x.HasValue).When(x => x.Student.ProgramId != null && x.Student.ProgramId > 0)
            .WithName("Sınav Dönemi").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        RuleFor(dto => dto.Student.BeginningYear).Must(x => x.HasValue).When(x => x.Student.ProgramId != null && x.Student.ProgramId > 0)
            .WithName("Sınav Yılı").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        RuleFor(dto => dto.Student.QuotaType).Must(x => x.HasValue).When(x => x.Student.ProgramId != null && x.Student.ProgramId > 0)
            .WithName("Kontenjan Türü").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        //RuleFor(dto => dto.Student.QuotaType_1).Must(x => x.HasValue).When(x => x.Student.ProgramId != null && x.Student.ProgramId > 0)
        //    .WithName("Kontenjan Türü-1").WithMessage(Resources.Validation.ValidationResource.RequiredField);

        //RuleFor(dto => dto.Student.QuotaType_2).Must(x => x.HasValue).When(x => x.Student.ProgramId != null && x.Student.ProgramId > 0 && x.Student.QuotaType_1 != QuotaType_1.YBU && x.Student.QuotaType_1 != QuotaType_1.ADL && x.Student.QuotaType_1 != QuotaType_1.GuestMilitaryPersonnel)
        //    .WithName("Kontenjan Türü-2").WithMessage(Resources.Validation.ValidationResource.RequiredField);
    }
}