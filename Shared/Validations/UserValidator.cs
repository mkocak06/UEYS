using FluentValidation;
using Shared.Models;
using Shared.RequestModels;
using System.Text.RegularExpressions;

namespace Shared.Validations
{
    public class UserValidator : AbstractValidator<UserForRegisterDTO>
    {
        public UserValidator()
        {
           
            RuleFor(user => user.Email).EmailAddress().NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithName(Resources.Validation.ValidationResource.Name).WithMessage(Resources.Validation.ValidationResource.RequiredField);
            //RuleFor(user => user.Address).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.BirthDate).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            RuleFor(user => user.BirthPlace).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            //RuleFor(user => user.Institution).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            //RuleFor(user => user.Province).NotEmpty().WithMessage(Resources.Validation.ValidationResource.NotBeEmpty);
            // RuleFor(user => user.InstitutionId).GreaterThan(0).WithMessage(Resources.Validation.ValidationResource.Required);
           // RuleFor(user => user.ProvinceId).GreaterThan(0).WithMessage(Resources.Validation.ValidationResource.Required);
            





        }
    }

}
