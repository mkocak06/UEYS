using FluentValidation;
using Shared.BaseModels;
using Shared.Resources.Validation;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Validations
{
    public class StudentRotationPerfectionBaseValidator<T> : AbstractValidator<T> where T : StudentRotationPerfectionBase
    {
        protected StudentRotationPerfectionBaseValidator() 
        {
            RuleFor(dto => dto.IsSuccessful).NotNull().WithMessage(ValidationResource.Required);
            RuleFor(dto => dto.ProcessDate).NotEmpty().WithMessage(ValidationResource.Required);
        }
    }

    public class StudentRotationPerfectionValidator : StudentRotationPerfectionBaseValidator<StudentRotationPerfectionResponseDTO>
    {
        public StudentRotationPerfectionValidator()
        {
            RuleFor(dto => dto.EducatorId).Must(s => s > 0).WithMessage(ValidationResource.Required);
        }
    }
}
