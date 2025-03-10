using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class PropertyBaseValidator<T> : AbstractValidator<T> where T : PropertyBase
{
    protected PropertyBaseValidator()
    {
       RuleFor(dto => dto.Name).Must(s => !string.IsNullOrWhiteSpace(s)).WithMessage(Resources.Validation.ValidationResource.Required);
       RuleFor(dto => dto.PropertyType).Must(s => s.HasValue && s.Value >= 0).WithMessage(Resources.Validation.ValidationResource.Required);
       RuleFor(dto => dto.PerfectionType).Must(s => s.HasValue && s.Value >= 0).WithMessage(Resources.Validation.ValidationResource.Required);//OLMASI GEREKEN BU

    }
}

public class PropertyValidator : PropertyBaseValidator<PropertyBase>
{

}
public class PropertyDTOValidator : PropertyBaseValidator<PropertyDTO>
{
}   