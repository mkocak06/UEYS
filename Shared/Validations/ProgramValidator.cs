using FluentValidation;
using Shared.BaseModels;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace Shared.Validations;

public class ProgramBaseValidator<T> : AbstractValidator<T> where T : ProgramBase
{
    protected ProgramBaseValidator()
    {
    }
}

public class ProgramValidator : ProgramBaseValidator<ProgramResponseDTO>
{
}

public class ProgramDTOValidator : ProgramBaseValidator<ProgramDTO>
{
}