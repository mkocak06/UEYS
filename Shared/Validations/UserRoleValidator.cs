
using FluentValidation;
using Shared.ResponseModels;
using Shared.Types;

namespace Shared.Validations
{
    public class UserRoleValidator : AbstractValidator<UserRolesModel>
    {
        public UserRoleValidator()
        {
            RuleFor(user => user.UserRoleList).Must(x => x != null && x.Count > 0).WithMessage("En az 1 rol girmelisiniz");
            RuleForEach(m => m.UserRoleList).ChildRules(ur =>
            {
                ur.RuleFor(x => x.Role).Must(r => r != null)
                    .WithName("Rol")
                    .WithMessage(Resources.Validation.ValidationResource.RequiredField);

                ur.RuleFor(x => x.UserRoleFaculties).Must(a => a != null && a.Count > 0)
                    .When(x => x.Role?.CategoryId == (long)RoleCategoryType.Faculty)
                    .WithName("Fakülte")
                    .WithMessage(Resources.Validation.ValidationResource.RequiredField);

                ur.RuleFor(x => x.UserRolePrograms).Must(a => a != null && a.Count > 0)
                    .When(x => x.Role?.CategoryId == (long)RoleCategoryType.Program)
                    .WithName("Program")
                    .WithMessage(Resources.Validation.ValidationResource.RequiredField);

                ur.RuleFor(x => x.UserRoleHospitals).Must(a => a != null && a.Count > 0)
                    .When(x => x.Role?.CategoryId == (long)RoleCategoryType.Hospital)
                    .WithName("Eğitim Verilen Kurum")
                    .WithMessage(Resources.Validation.ValidationResource.RequiredField);

                ur.RuleFor(x => x.UserRoleUniversities).Must(a => a != null && a.Count > 0)
                    .When(x => x.Role?.CategoryId == (long)RoleCategoryType.University)
                    .WithName("Üniversite")
                    .WithMessage(Resources.Validation.ValidationResource.RequiredField);

                ur.RuleFor(x => x.UserRoleProvinces).Must(a => a != null && a.Count > 0)
                    .When(x => x.Role?.CategoryId == (long)RoleCategoryType.Province)
                    .WithName("İl")
                    .WithMessage(Resources.Validation.ValidationResource.RequiredField);
            });


        }
    }
}